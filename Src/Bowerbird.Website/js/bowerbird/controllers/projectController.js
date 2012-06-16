/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ProjectController & ProjectRouter
// ---------------------------------
define(['jquery', 'underscore', 'backbone', 'app', 'views/projectformlayoutview', 'collections/exploreprojectcollection', 'models/project'],
function ($, _, Backbone, app, ProjectFormLayoutView, ExploreProjectCollection, Project) 
{
    var ProjectRouter = Backbone.Marionette.AppRouter.extend({
        appRoutes: {
            'projects/explore': 'showProjectExplorer',
            'projects/create': 'showProjectForm',
            'projects/:id/update': 'showProjectForm'
        }
    });

    var ProjectController = {};

    var getModel = function (id) {
        var deferred = new $.Deferred();

        if (app.isPrerendering('projects')) {
            deferred.resolve(app.prerenderedView.data);
        } else {
            var params = {};
            if (id) {
                params['id'] = id;
            }
            $.ajax({
                url: '/projects/create',
                data: params
            }).done(function (data) {
                deferred.resolve(data.Model);
            });
        }

        return deferred.promise();
    };

    //    var getExploreList = function (page, pageSize, sortField, sortDirection, searchQuery) {
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

    // Show an project form
    ProjectController.showProjectForm = function (id) {
        log('projectController:showProjectForm');
        $.when(getModel(id))
            .done(function (model) {
                var project = new Project(model.Project);
                var projectFormLayoutView = new ProjectFormLayoutView({ model: project, teams: model.Teams });

                //app.content[app.getShowViewMethodName('projects')](projectFormLayoutView);
                app.showFormContentView(projectFormLayoutView, 'projects');

                if (app.isPrerendering('projects')) {
                    projectFormLayoutView.showBootstrappedDetails();
                }

                app.setPrerenderComplete();
            });
    };

    // Show an project form
    //ProjectController.showProjectExplorer = function (page, pageSize, sortField, sortDirection, searchQuery) {
    ProjectController.showProjectExplorer = function () {
        log('projectController:showProjects');
        //$.when(getExploreList(page, pageSize, sortField, sortDirection, searchQuery))
        $.when(getExploreList())
            .done(function (model) {
                log('got to the ProjectController.showProjects function');

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

    app.addInitializer(function () {
        this.projectRouter = new ProjectRouter({
            controller: ProjectController
        });
    });

    return ProjectController;

});