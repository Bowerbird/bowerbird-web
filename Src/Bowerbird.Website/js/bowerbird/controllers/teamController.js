/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// TeamController & TeamRouter
// ---------------------------
define(['jquery','underscore','backbone','app','models/team','views/teamformlayoutview'],
function ($, _, Backbone, app, Team, TeamFormLayoutView) 
{
    var TeamRouter = Backbone.Marionette.AppRouter.extend({
        appRoutes: {
            'teams/create': 'showTeamForm'
        }
    });

    var TeamController = {};

    var getModel = function (id) {
        var deferred = new $.Deferred();

        if (app.isPrerendering('teams')) {
            deferred.resolve(app.prerenderedView.data);
        } else {
            var params = {};
            if (id) {
                params['id'] = id;
            }
            $.ajax({
                url: '/teams/create',
                data: params
            }).done(function (data) {
                deferred.resolve(data.Model);
            });
        }
        return deferred.promise();
    };

    // TeamController Public API
    // ---------------------------------

    // Show an team form
    TeamController.showTeamForm = function (id) {
        log('teamController:showTeamForm');
        $.when(getModel(id))
            .done(function (model) {
                var team = new Team(model.Team);
                var teamFormLayoutView = new TeamFormLayoutView({ model: team, organisations: model.Organisations });

                app.content[app.getShowViewMethodName('teams')](teamFormLayoutView);

                if (app.isPrerendering('teams')) {
                    teamFormLayoutView.showBootstrappedDetails();
                }

                app.setPrerenderComplete();
            });
    };

    app.addInitializer(function () {
        this.teamRouter = new TeamRouter({
            controller: TeamController
        });
    });

    return TeamController;

});