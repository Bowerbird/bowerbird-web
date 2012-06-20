/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ProjectController & ProjectRouter
// ---------------------------------
define(['jquery', 'underscore', 'backbone', 'app', 'views/projectlayoutview', 'views/projectformlayoutview', 'views/projectcollectionview', 'models/project', 'collections/projectcollection'],
function ($, _, Backbone, app, ProjectLayoutView, ProjectFormLayoutView, ProjectCollectionView, Project, ProjectCollection) {

    var ProjectRouter = Backbone.Marionette.AppRouter.extend({
        appRoutes: {
            'projects/explore': 'showProjectExplorer',
            'projects/:id/update': 'showProjectForm',
            'projects/create': 'showProjectForm',
            'projects/:id': 'showProjectStream'
        }
    });

    var ProjectController = {};

    app.vent.on('joinProject:', function (project) {
        ProjectController.joinProject(project);
    });

    var getModel = function (id) {
        var deferred = new $.Deferred();

        if (app.isPrerendering('projects')) {
            deferred.resolve(app.prerenderedView.data);
        } else {
            var params = {};
            if (id) {
                params['id'] = id;
                $.ajax({
                    url: '/projects/' + id,
                    data: params
                }).done(function (data) {
                    deferred.resolve(data.Model);
                });
            }
            else {
                $.ajax({
                    url: '/projects/create'
                }).done(function (data) {
                    deferred.resolve(data.Model);
                });
            }
        }

        return deferred.promise();
    };

    var getExploreList = function (page, pageSize, sortField, sortDirection, searchQuery) {
        var deferred = new $.Deferred();

        if (app.isPrerendering('projects')) {
            deferred.resolve(app.prerenderedView.data);
        } else {
            var params = {};
            //            if (page) {
            //                params['page'] = page;
            //            }
            //            if (pageSize) {
            //                params['pageSize'] = pageSize;
            //            }
            //            if (sortField) {
            //                params['sortField'] = sortField;
            //            }
            //            if (sortDirection) {
            //                params['sortDirection'] = sortDirection;
            //            }
            //            if (searchQuery) {
            //                params['searchQuery'] = searchQuery;
            //            }
            $.ajax({
                url: '/projects/explore',
                data: params
            }).done(function (data) {
                deferred.resolve(data.Model);
            });
        }

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
                var projectLayoutView = new ProjectLayoutView({ model: project });

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
                var projectFormLayoutView = new ProjectFormLayoutView({ model: project, teams: model.Teams });

                app.showFormContentView(projectFormLayoutView, 'projects');

                if (app.isPrerendering('projects')) {
                    projectFormLayoutView.showBootstrappedDetails();
                }

                app.setPrerenderComplete();
            });
    };

    // Show an project form
    ProjectController.showProjectExplorer = function () {
        log('projectController:showProjects');
        $.when(getExploreList())
            .done(function (model) {
                var projectCollection = new ProjectCollection(model.Projects.PagedListItems);

                var projectCollectionView = new ProjectCollectionView({ collection: projectCollection });

                //app.showFormContentView(projectCollectionView, 'projectexplorer');
                app.content[app.getShowViewMethodName('projects')](projectCollectionView);

                if (app.isPrerendering('projectexplorer')) {
                    projectCollectionView.showBootstrappedDetails();
                }

                app.setPrerenderComplete();
                //projectLayoutView.showStream(new StreamItemCollection(app.prerenderedView.data.StreamItems.PagedListItems));

                //                var projects = new PaginatedCollection(model.Project);
                //                var projectFormLayoutView = new ProjectFormLayoutView({ model: project, teams: model.Teams });

                //                app.content[app.getShowViewMethodName('projects')](projectFormLayoutView);

                //                if (app.isPrerendering('projects')) {
                //                    projectFormLayoutView.showBootstrappedDetails();
                //                }

                //                app.setPrerenderComplete();
            });
    };

    // Join a project
    ProjectController.joinProject = function (project) {
        var deferred = new $.Deferred();
        log('projectController.joinProject');
        var params = {};
        if (project) {
            params['Id'] = project.id;
        }
        $.ajax({
            url: '/projects/join',
            data: JSON.stringify(params),
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            type: 'POST'
        })
        .done(function (data) {
            deferred.resolve(data.Model);
        });

        return deferred.promise();
    };

    app.addInitializer(function () {
        this.projectRouter = new ProjectRouter({
            controller: ProjectController
        });
    });

    return ProjectController;
});