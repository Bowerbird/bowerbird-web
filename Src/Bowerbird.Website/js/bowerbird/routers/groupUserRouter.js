/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// Bowerbird.Routers.GroupUserRouter
// ---------------------------------
define(['jquery', 'underscore', 'backbone', 'app', 'controllers/groupusercontroller'], function ($, _, Backbone, app, GroupUserController) {

    var GroupUserRouter = Backbone.Marionette.AppRouter.extend({
        appRoutes: {
            '': 'showHome',
            'teams/:id': 'showTeam',
            'projects/:id': 'showProjectStream',
            'projects/:id/about': 'showProjectAbout',
            'projects/:id/members': 'showProjectMembers',
            'users/:id': 'showUser'
        }
    });

    app.addInitializer(function () {
        this.groupUserRouter = new GroupUserRouter({
            controller: GroupUserController
        });
    });

    return GroupUserRouter;

});
