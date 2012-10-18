/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// HomeController & HomeRouter
// ---------------------------
define(['jquery', 'underscore', 'backbone', 'app', 'views/homepublicview', 'views/homeprivateview'],
function ($, _, Backbone, app, HomePublicView, HomePrivateView) {
    var HomeRouter = Backbone.Marionette.AppRouter.extend({
        appRoutes: {
            '': 'showActivity',
            'sightings': 'showSightings',
            'all': 'showAllBowerbirdActivity',
            'favourites': 'showFavourites'
        }
    });

    var HomeController = {};

    var getModel = function (uri, action) {
        var deferred = new $.Deferred();
        if (app.isPrerendering('home')) {
            deferred.resolve(app.prerenderedView.data);
        } else {
            $.ajax({
                url: uri,
                type: action || 'GET'
            }).done(function (data) {
                deferred.resolve(data.Model);
            });
        }
        return deferred.promise();
    };

    // Public API
    // ----------

    HomeController.showActivity = function () {
        $.when(getModel('/'))
            .done(function (model) {
                if (app.content.currentView instanceof HomePrivateView) {
                    app.content.currentView.showActivity(model.Activities);
                } else {
                    var homeView;

                    if (app.authenticatedUser) {
                        var options = { model: app.authenticatedUser.user };
                        if (app.isPrerendering('home')) {
                            options['el'] = '.home-private';
                        }
                        homeView = new HomePrivateView(options);
                    } else {
                        homeView = new HomePublicView();
                    }

                    app.showContentView('', homeView, 'home', function () {
                        if (app.authenticatedUser) {
                            homeView.showActivity(model.Activities);
                        }
                    });
                }
            });
    };

    HomeController.showSightings = function () {
        $.when(getModel('/sightings'))
            .done(function (model) {
                if (app.content.currentView instanceof HomePrivateView) {
                    app.content.currentView.showSightings(model.Sightings);
                } else {
                    var homeView;

                    if (app.authenticatedUser) {
                        var options = { model: app.authenticatedUser.user };
                        if (app.isPrerendering('home')) {
                            options['el'] = '.home-private';
                        }
                        homeView = new HomePrivateView(options);
                    } else {
                        homeView = new HomePublicView();
                    }

                    app.showContentView('', homeView, 'home', function () {
                        if (app.authenticatedUser) {
                            homeView.showSightings(model.Sightings);
                        }
                    });
                }
            });
    };

    HomeController.showAllBowerbirdActivity = function () {
    };

    HomeController.showFavourites = function () {
    };

    app.addInitializer(function () {
        this.homeRouter = new HomeRouter({
            controller: HomeController
        });
    });

    return HomeController;

});