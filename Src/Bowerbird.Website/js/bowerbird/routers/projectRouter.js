/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ProjectRouter
// ------------------

define(['jquery', 'underscore', 'backbone', 'app', 'controllers/projectcontroller'], function ($, _, Backbone, app, ProjectController) {

    var ProjectRouter = Backbone.Marionette.AppRouter.extend({
        appRoutes: {
            'projects/create': 'showProjectForm',
            'projects/:id/update': 'showProjectForm'
        }
    });

    app.addInitializer(function () {
        this.projectRouter = new ProjectRouter({
            controller: ProjectController
        });
    });

    return ProjectRouter;

});
