/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// TeamController
// ----------------------

// This is the controller contributions (observations & posts). It contains all of the 
// high level knowledge of how to run the app when it's in contribution mode.
define(['jquery', 'underscore', 'backbone', 'app', 'models/team', 'views/teamformlayoutview'], function ($, _, Backbone, app, Team, TeamFormLayoutView) {

    var TeamController = {};

    // TeamController Public API
    // ---------------------------------

    // Show a team
    TeamController.showTeamForm = function () {

        var teamFormLayoutView = new TeamFormLayoutView({ el: $('.team-create-form'), model: new Team(app.prerenderedView.Team) });

        teamFormLayoutView.render();

        app.prerenderedView.isBound = true;
    };

    // TeamController Event Handlers
    // -------------------------------------

    app.vent.on('team:show', function () {
        TeamController.showTeamForm();
    });

    return TeamController;

});