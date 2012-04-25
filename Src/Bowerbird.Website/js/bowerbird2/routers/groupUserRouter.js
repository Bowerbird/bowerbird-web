/// <reference path="../libs/log.js" />
/// <reference path="../libs/jquery-1.7.1.min.js" />
/// <reference path="../libs/underscore.js" />
/// <reference path="../libs/backbone.js" />
/// <reference path="../libs/backbone.marionette.js" />
/// <reference path="groupUserController.js" />

// Bowerbird.Routers.GroupUserRouter
// ---------------------------------
Bowerbird.Routers.GroupUserRouter = (function (app, Bowerbird, Backbone, $, _) {

    var GroupUserRouter = Backbone.Marionette.AppRouter.extend({
        appRoutes: {
            '': 'showHome',
            'teams/:id': 'showTeam',
            'projects/:id': 'showProject',
            'users/:id': 'showUser'
        }
    });

    app.addInitializer(function () {
        Bowerbird.app.groupUserRouter = new GroupUserRouter({
            controller: Bowerbird.Controllers.GroupUserController
        });
    });

    return GroupUserRouter;

})(Bowerbird.app, Bowerbird, Backbone, jQuery, _);
