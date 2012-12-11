/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ProjectController & ProjectRouter
// ---------------------------------
define(['jquery', 'underscore', 'backbone', 'app', 'models/project', 'collections/projectcollection', 'collections/activitycollection', 'collections/sightingcollection', 'views/projectdetailsview', 'views/projectformview', 'views/projectexploreview'],
function ($, _, Backbone, app, Project, ProjectCollection, ActivityCollection, SightingCollection, ProjectDetailsView, ProjectFormView, ProjectExploreView) {

    var ProjectRouter = Backbone.Marionette.AppRouter.extend({
        appRoutes: {
            'projects': 'showProjectExplore',
            'projects/create*': 'showProjectCreateForm',
            'projects/:id/sightings*': 'showSightings',
            'projects/:id/update': 'showProjectUpdateForm',
            'projects/:id': 'showProjectDetails'
        }
    });

    var ProjectController = {};

    var getModel = function (uri, action) {
        var deferred = new $.Deferred();
        if (app.isPrerenderingView('projects')) {
            deferred.resolve(app.prerenderedView.data);
        } else {
            $.ajax({
                url: uri,
                type: action || 'GET'
            }).done(function (data) {
                deferred.resolve(data.Model);
            });
        }
        return deferred.promise();
    };

    // Show project form
    var showProjectForm = function (uri) {
        $.when(getModel(uri))
            .done(function (model) {
                var project = new Project(model.Project);

                var options = { model: project, teams: model.Teams };

                if (app.isPrerenderingView('projects')) {
                    options['el'] = '.project-form';
                }

                var projectFormView = new ProjectFormView(options);
                app.showContentView('Edit Project', projectFormView, 'projects');
            });
    };

    // Public API
    // ----------

    // Show project details
    ProjectController.showProjectDetails = function (id) {
        // Beacause IE is using has fragments, we have to fix the id manually for IE
        var url = id;
        if (url.indexOf('projects') == -1) {
            url = '/projects/' + url;
        }

        $.when(getModel(url))
            .done(function (model) {
                var project = new Project(model.Project);
                var activityCollection = new ActivityCollection(model.Activities.PagedListItems, { id: project.id });
                activityCollection.setPageInfo(model.Activities);

                if (app.content.currentView instanceof ProjectDetailsView && app.content.currentView.model.id === project.id) {
                    app.content.currentView.showActivity(activityCollection);
                } else {
                    var options = { model: project };
                    if (app.isPrerenderingView('projects')) {
                        options['el'] = '.project';
                    }
                    var projectDetailsView = new ProjectDetailsView(options);

                    app.showContentView(project.get('Name'), projectDetailsView, 'projects', function () {
                        projectDetailsView.showActivity(activityCollection);
                    });
                }
            });
    };

    ProjectController.showSightings = function (id, params) {
        $.when(getModel('/projects/' + id + '/sightings?view=' + (params && params.view ? params.view : 'thumbnails') + '&sort=' + (params && params.sort ? params.sort : 'latestadded')))
            .done(function (model) {
                var project = new Project(model.Project);
                var sightingCollection = new SightingCollection(model.Sightings.PagedListItems, { projectId: project.id, page: model.Query.page, pageSize: model.Query.PageSize, total: model.Sightings.TotalResultCount, viewType: model.Query.View, sortBy: model.Query.Sort });

                if (app.content.currentView instanceof ProjectDetailsView && app.content.currentView.model.id === project.id) {
                    app.content.currentView.showSightings(sightingCollection);
                } else {
                    var options = { model: project };
                    if (app.isPrerenderingView('projects')) {
                        options['el'] = '.project';
                    }
                    var projectDetailsView = new ProjectDetailsView(options);

                    app.showContentView(project.get('Name'), projectDetailsView, 'projects', function () {
                        projectDetailsView.showSightings(sightingCollection);
                    });
                }
            });
    };

    // Show project create form
    ProjectController.showProjectCreateForm = function (id) {
        var uri = '/projects/create';
        if (id) {
            uri += id;
        }
        showProjectForm(uri);
    };

    // Show project update form
    ProjectController.showProjectUpdateForm = function (id) {
        showProjectForm('/projects/' + id + '/update');
    };

    // Show project explore
    ProjectController.showProjectExplore = function () {
        $.when(getModel('/projects'))
            .done(function (model) {
                var projectCollection = new ProjectCollection(model.Projects.PagedListItems);
                var projectExploreView = new ProjectExploreView({ model: app.authenticatedUser.user, collection: projectCollection });
                app.showContentView('Projects', projectExploreView, 'projects');
            });
    };

    // Event Handlers
    // --------------

    app.vent.on('joinProject', function (project) {
        $.when(getModel('/' + project.id + '/join', 'POST'));
    });

    app.vent.on('leaveProject', function (project) {
        $.when(getModel('/' + project.id + '/leave', 'POST'))
            .done(function (model) {
                app.authenticatedUser.projects.remove(project.id);
            });
    });

    app.vent.on('projectAdded:', function (project) {
        if (ProjectController.projectCollection) {
            ProjectController.projectCollection.add(project);
        }
    });

    app.addInitializer(function () {
        this.projectRouter = new ProjectRouter({
            controller: ProjectController
        });
    });

    return ProjectController;
});