/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// TeamController & TeamRouter
// ---------------------------
define(['jquery','underscore','backbone','app','models/team','views/teamformlayoutview', 'views/teamlayoutview', 'views/teamcollectionview', 'collections/teamcollection'],
function ($, _, Backbone, app, Team, TeamFormLayoutView, TeamLayoutView, TeamCollectionView, TeamCollection) 
{
    var TeamRouter = Backbone.Marionette.AppRouter.extend({
        appRoutes: {
            'teams/explore': 'showTeamExplorer',
            'teams/:id/update': 'showTeamForm',
            'teams/create': 'showTeamForm',
            'teams/:id': 'showTeamStream'
        }
    });

    var TeamController = {};

    app.vent.on('teamAdded:', function (team) {
        if (TeamController.teamCollection) {
            TeamController.teamCollection.add(team);
        }
    });

    app.vent.on('viewTeam:', function (team) {
        TeamController.showTeamStream(team.id);
    });

    var getModel = function (id) {
        var deferred = new $.Deferred();
        if (app.isPrerendering('teams')) {
            deferred.resolve(app.prerenderedView.data);
        } else {
            var params = {};
            if (id) {
                params['id'] = id;
                $.ajax({
                    url: '/teams/' + id,
                    data: params
                }).done(function (data) {
                    deferred.resolve(data.Model);
                });
            }
            else {
                $.ajax({
                    url: '/teams/create'
                }).done(function (data) {
                    deferred.resolve(data.Model);
                });
            }
        }
        return deferred.promise();
    };

    var getExploreList = function (page, pageSize, sortField, sortDirection, searchQuery) {
        var deferred = new $.Deferred();
        if (app.isPrerendering('teams')) {
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
                url: '/teams/explore',
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

                //app.content[app.getShowViewMethodName('teams')](teamFormLayoutView);
                app.showFormContentView(teamFormLayoutView, 'teams');

                if (app.isPrerendering('teams')) {
                    teamFormLayoutView.showBootstrappedDetails();
                }

                app.setPrerenderComplete();
            });
    };

    // Show team activity
    TeamController.showTeamStream = function (id) {
        log('showing teams home stream', this, this);
        $.when(getModel(id))
        .done(function (model) {
            var team = new Team(model.Team);
            log('HACK: injected teams/ into team id value');
            team.set('Id', 'teams/' + id);
            var teamLayoutView = new TeamLayoutView({ model: team });
            app.showFormContentView(teamLayoutView, 'teams');
            if (app.isPrerendering('teams')) {
                teamLayoutView.showBootstrappedDetails();
            }
            teamLayoutView.showStream();
            app.setPrerenderComplete();
        });
    };

    // Show an team explore
    TeamController.showTeamExplorer = function () {
        log('teamController:showTeams');
        $.when(getExploreList())
            .done(function (model) {
                TeamController.teamCollection = new TeamCollection(model.Teams.PagedListItems);
                var teamCollectionView = new TeamCollectionView({ collection: TeamController.teamCollection });
                //app.content[app.getShowViewMethodName('teams')](teamCollectionView);
                app.showFormContentView(teamCollectionView, 'teams');
                if (app.isPrerendering('teams')) {
                    teamCollectionView.showBootstrappedDetails();
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