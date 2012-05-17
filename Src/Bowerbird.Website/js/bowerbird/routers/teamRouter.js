/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// TeamRouter
// ------------------

define(['jquery', 'underscore', 'backbone', 'app', 'controllers/teamcontroller'], function ($, _, Backbone, app, TeamController) {

    var TeamRouter = Backbone.Marionette.AppRouter.extend({
        appRoutes: {
            'teams/create': 'showTeamForm'
        }
    });

    app.addInitializer(function () {
        this.teamRouter = new TeamRouter({
            controller: TeamController
        });
    });

    return TeamRouter;
});