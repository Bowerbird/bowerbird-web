/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ProjectController & ProjectRouter
// ---------------------------------
define(['jquery', 'underscore', 'backbone', 'app', 'views/projectlayoutview', 'views/projectformlayoutview', 'views/projectscompositeview', 'models/project', 'collections/projectcollection'],
function ($, _, Backbone, app, ProjectLayoutView, ProjectFormLayoutView, ProjectsCompositeView, Project, ProjectCollection) {

    var ProjectRouter = Backbone.Marionette.AppRouter.extend({
        appRoutes: {
            'projects': 'showProjectExplorer',
            'projects/create': 'showProjectForm',
            'projects/:id/update': 'showProjectForm',
            'projects/:id': 'showProjectStream'
        }
    });

    var ProjectController = {};

    var getModel = function (id) {
        var url = '/projects/create';
        if (id) {
            url = id;
        }
        var deferred = new $.Deferred();
        if (app.isPrerendering('projects')) {
            deferred.resolve(app.prerenderedView.data);
        } else {
            $.ajax({
                url: url
            }).done(function (data) {
                deferred.resolve(data.Model);
            });
        }
        return deferred.promise();
    };

    var getExploreList = function (page, pageSize, sortField, sortDirection, searchQuery) {
        var deferred = new $.Deferred();
        if (app.isPrerendering('projects')) {
            deferred.resolve(app.prerenderedView.data);
        } else {
            var params = {};
            $.ajax({
                url: '/projects',
                data: params
            }).done(function (data) {
                deferred.resolve(data.Model);
            });
        }
        return deferred.promise();
    };

    // Join a project
    var joinProject = function (project) {
        var deferred = new $.Deferred();
        $.ajax({
            url: '/' + project.id + '/join',
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            type: 'POST'
        })
        .done(function (data) {
            deferred.resolve(data.Model);
        });

        return deferred.promise();
    };

    // Leave a project
    var leaveProject = function (project) {
        var deferred = new $.Deferred();
        $.ajax({
            url: '/' + project.id + '/leave',
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            type: 'POST'
        })
        .done(function (data) {
            deferred.resolve(data.Model);
            app.authenticatedUser.projects.remove(project.id);
        });

        return deferred.promise();
    };

    // ProjectController Public API
    // ----------------------------

    // Show project activity
    ProjectController.showProjectStream = function (id) {
        log('showing projects home stream', this, this);
        $.when(getModel(id))
            .done(function (model) {
                var project = new Project(model.Project);
                app.updateTitle(project.get('Name'));
                var projectLayoutView = new ProjectLayoutView({ model: project });
                //app.showFormContentView(projectLayoutView, 'projects');
                app.content[app.getShowViewMethodName('projects')](projectLayoutView);
                if (app.isPrerendering('projects')) {
                    projectLayoutView.showBootstrappedDetails();
                }
                projectLayoutView.showStream();
                app.setPrerenderComplete();
            });
    };

    // Show an project form
    ProjectController.showProjectForm = function (id) {
        log('projectController:showProjectForm');
        $.when(getModel(id))
            .done(function (model) {
                var project = new Project(model.Project);
                if (project.id) {
                    app.updateTitle('Edit Project');
                } else {
                    app.updateTitle('New Project');
                }
                var projectFormLayoutView = new ProjectFormLayoutView({ model: project, teams: model.Teams });
                app.showFormContentView(projectFormLayoutView, 'projects');
                if (app.isPrerendering('projects')) {
                    projectFormLayoutView.showBootstrappedDetails();
                }
                app.setPrerenderComplete();
            });
    };

    // Show an project explore
    ProjectController.showProjectExplorer = function () {
        log('projectController:showProjects');
        $.when(getExploreList())
            .done(function (model) {
                app.updateTitle('Projects');
                var projectCollection = new ProjectCollection(model.Projects.PagedListItems);
                var projectsCompositeView = new ProjectsCompositeView({ model: app.authenticatedUser.user, collection: projectCollection });
                //var projectCollectionView = new ProjectCollectionView({ collection: ProjectController.projectCollection });
                //app.showFormContentView(projectCollectionView, 'projects');

                app.content[app.getShowViewMethodName('projects')](projectsCompositeView);
                if (app.isPrerendering('projects')) {
                    projectsCompositeView.showBootstrappedDetails();
                }
                app.setPrerenderComplete();
            });
    };

    app.vent.on('joinProject', function (project) {
        joinProject(project);
    });

    app.vent.on('leaveProject', function (project) {
        leaveProject(project);
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