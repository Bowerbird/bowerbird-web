/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ProjectController & ProjectRouter
// ---------------------------------
define(['jquery', 'underscore', 'backbone', 'app', 'models/project', 'collections/projectcollection', 'collections/activitycollection', 'collections/sightingcollection', 'collections/postcollection',
        'collections/usercollection', 'views/projectdetailsview', 'views/projectformview', 'views/projectexploreview'],
function ($, _, Backbone, app, Project, ProjectCollection, ActivityCollection, SightingCollection, PostCollection, UserCollection, ProjectDetailsView, ProjectFormView, ProjectExploreView) {

    var ProjectRouter = Backbone.Marionette.AppRouter.extend({
        appRoutes: {
            'projects/create*': 'showCreateForm',
            'projects/:id/sightings*': 'showSightings',
            'projects/:id/posts*': 'showPosts',
            'projects/:id/members*': 'showMembers',
            'projects/:id/about': 'showAbout',
            'projects/:id/update': 'showUpdateForm',
            'projects/:id': 'showProjectDetails',
            'projects*': 'showExplore'
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

                var options = {
                    model: project,
                    categoriesSelectList: model.CategoriesSelectList
                };

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
        $.when(getModel('/projects/' + id + '/sightings?view=' + (params && params.view ? params.view : 'thumbnails') + '&sort=' + (params && params.sort ? params.sort : 'newest')))
            .done(function (model) {
                var project = new Project(model.Project);
                var sightingCollection = new SightingCollection(model.Sightings.PagedListItems,
                    { 
                        projectId: project.id,
                        page: model.Query.page,
                        pageSize: model.Query.PageSize,
                        total: model.Sightings.TotalResultCount,
                        viewType: model.Query.View,
                        sortBy: model.Query.Sort,
                        category: model.Query.Category,
                        needsId: model.Query.NeedsId,
                        query: model.Query.Query,
                        field: model.Query.Field,
                        taxonomy: model.Query.Taxonomy
                    });

                if (app.content.currentView instanceof ProjectDetailsView && app.content.currentView.model.id === project.id) {
                    app.content.currentView.showSightings(sightingCollection, model.CategorySelectList, model.FieldSelectList);
                } else {
                    var options = { model: project };
                    if (app.isPrerenderingView('projects')) {
                        options['el'] = '.project';
                    }
                    var projectDetailsView = new ProjectDetailsView(options);

                    app.showContentView(project.get('Name'), projectDetailsView, 'projects', function () {
                        projectDetailsView.showSightings(sightingCollection, model.CategorySelectList, model.FieldSelectList);
                    });
                }
            });
    };

    ProjectController.showPosts = function (id, params) {
        $.when(getModel('/projects/' + id + '/posts?view=' + '&sort=' + (params && params.sort ? params.sort : 'newest')))
        .done(function (model) {
            var project = new Project(model.Project);
            var postCollection = new PostCollection(model.Posts.PagedListItems,
                { 
                    groupId: project.id,
                    page: model.Query.page,
                    pageSize: model.Query.PageSize,
                    total: model.Posts.TotalResultCount,
                    sortBy: model.Query.Sort,
                    query: model.Query.Query,
                    field: model.Query.Field
                });

            if (app.content.currentView instanceof ProjectDetailsView && app.content.currentView.model.id === project.id) {
                app.content.currentView.showPosts(postCollection, model.FieldSelectList);
            } else {
                var options = { model: project };
                if (app.isPrerenderingView('projects')) {
                    options['el'] = '.project';
                }
                var projectDetailsView = new ProjectDetailsView(options);

                app.showContentView(project.get('Name'), projectDetailsView, 'projects', function () {
                    projectDetailsView.showPosts(postCollection, model.FieldSelectList);
                });
            }
        });
    };

    ProjectController.showMembers = function (id, params) {
        $.when(getModel('/projects/' + id + '/members?sort=' + (params && params.sort ? params.sort : 'a-z')))
        .done(function (model) {
            var project = new Project(model.Project);
            var userCollection = new UserCollection(model.Members.PagedListItems, { projectId: project.id, page: model.Query.page, pageSize: model.Query.PageSize, total: model.Members.TotalResultCount, viewType: model.Query.View, sortBy: model.Query.Sort });

            if (app.content.currentView instanceof ProjectDetailsView && app.content.currentView.model.id === project.id) {
                app.content.currentView.showMembers(userCollection);
            } else {
                var options = { model: project };
                if (app.isPrerenderingView('projects')) {
                    options['el'] = '.project';
                }
                var projectDetailsView = new ProjectDetailsView(options);

                app.showContentView(project.get('Name'), projectDetailsView, 'projects', function () {
                    projectDetailsView.showMembers(userCollection);
                });
            }
        });
    };

    ProjectController.showAbout = function (id) {
        $.when(getModel('/projects/' + id + '/about'))
        .done(function (model) {
            var project = new Project(model.Project);

            if (app.content.currentView instanceof ProjectDetailsView && app.content.currentView.model.id === project.id) {
                app.content.currentView.showAbout(model.ProjectAdministrators, model.ActivityTimeseries);
            } else {
                var options = { model: project };
                if (app.isPrerenderingView('projects')) {
                    options['el'] = '.project';
                }
                var projectDetailsView = new ProjectDetailsView(options);

                app.showContentView(project.get('Name'), projectDetailsView, 'projects', function () {
                    projectDetailsView.showAbout(model.ProjectAdministrators, model.ActivityTimeseries);
                });
            }
        });
    };

    // Show project create form
    ProjectController.showCreateForm = function (id) {
        var uri = '/projects/create';
        if (id) {
            uri += id;
        }
        showProjectForm(uri);
    };

    // Show project update form
    ProjectController.showUpdateForm = function (id) {
        showProjectForm('/projects/' + id + '/update');
    };

    // Show project explore
    ProjectController.showExplore = function (params) {
        $.when(getModel('/projects?sort=' + (params && params.sort ? params.sort : 'popular')))
        .done(function (model) {
            var projectCollection = new ProjectCollection(model.Projects.PagedListItems,
                {
                    page: model.Query.page,
                    pageSize: model.Query.PageSize,
                    total: model.Projects.TotalResultCount,
                    viewType: model.Query.View,
                    sortBy: model.Query.Sort,
                    category: model.Query.Category,
                    query: model.Query.Query,
                    field: model.Query.Field
                });

            var options = {
                projectCollection: projectCollection,
                categorySelectList: model.CategorySelectList,
                fieldSelectList: model.FieldSelectList
            };
            
            if (app.authenticatedUser) {
                options.model = app.authenticatedUser.user;
            }

            if (app.isPrerenderingView('projects')) {
                options['el'] = '.projects';
            }
            var projectExploreView = new ProjectExploreView(options);

            app.showContentView('Projects', projectExploreView, 'projects', function () {
            });
        });
    };

    // Event Handlers
    // --------------

    app.vent.on('joinProject', function (project) {
        $.when(getModel('/' + project.id + '/members', 'POST'));
    });

    app.vent.on('leaveProject', function (project) {
        $.when(getModel('/' + project.id + '/members', 'DELETE'))
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