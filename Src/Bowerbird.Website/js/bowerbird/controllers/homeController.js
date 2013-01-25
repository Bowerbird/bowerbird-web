/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// HomeController & HomeRouter
// ---------------------------
define(['jquery', 'underscore', 'backbone', 'app', 'collections/sightingcollection', 'collections/postcollection', 'collections/activitycollection', 'views/homepublicview', 'views/homeprivateview'],
function ($, _, Backbone, app, SightingCollection, PostCollection, ActivityCollection, HomePublicView, HomePrivateView) {
    var HomeRouter = Backbone.Marionette.AppRouter.extend({
        appRoutes: {
            '': 'showHome',
            'home/sightings*': 'showSightings',
            'home/posts*': 'showPosts',
            'timeline': 'showAllBowerbirdActivity',
            'favourites': 'showFavourites'
        }
    });

    var HomeController = {};

    var getModel = function (uri, action) {
        var deferred = new $.Deferred();
        if (app.isPrerenderingView('home')) {
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

    HomeController.showHome = function () {
        $.when(getModel('/'))
            .done(function (model) {
                if (app.content.currentView instanceof HomePrivateView) {
                    var activityCollection = new ActivityCollection(model.Activities.PagedListItems);
                    activityCollection.setPageInfo(model.Activities);                    
                    app.content.currentView.showActivity(activityCollection);
                } else {
                    var homeView;
                    var options = {};
                    if (app.authenticatedUser) {
                        options = { model: app.authenticatedUser.user };
                        if (app.isPrerenderingView('home')) {
                            options['el'] = '.home-private';
                        }
                        homeView = new HomePrivateView(options);
                    } else {
                        if (app.isPrerenderingView('home')) {
                            options['el'] = '.home-public';
                        }
                        homeView = new HomePublicView(options);
                    }

                    app.showContentView('', homeView, 'home', function () {
                        if (app.authenticatedUser) {
                            var activityCollection = new ActivityCollection(model.Activities.PagedListItems);
                            activityCollection.setPageInfo(model.Activities);                            
                            homeView.showActivity(activityCollection);
                        }
                    });
                }
            });
    };

    HomeController.showSightings = function (params) {
        $.when(getModel('/home/sightings?view=' + (params && params.view ? params.view : 'thumbnails') + '&sort=' + (params && params.sort ? params.sort : 'newest')))
            .done(function (model) {
                var sightingCollection = new SightingCollection(model.Sightings.PagedListItems, { page: model.Query.page, pageSize: model.Query.PageSize, total: model.Sightings.TotalResultCount, viewType: model.Query.View, sortBy: model.Query.Sort });

                if (app.content.currentView instanceof HomePrivateView) {
                    app.content.currentView.showSightings(sightingCollection);
                } else {
                    var homeView;

                    if (app.authenticatedUser) {
                        var options = { model: app.authenticatedUser.user };
                        if (app.isPrerenderingView('home')) {
                            options['el'] = '.home-private';
                        }
                        homeView = new HomePrivateView(options);
                    } else {
                        homeView = new HomePublicView();
                    }

                    app.showContentView('', homeView, 'home', function () {
                        if (app.authenticatedUser) {
                            homeView.showSightings(sightingCollection);
                        }
                    });
                }
            });
    };

    HomeController.showPosts = function (id, params) {
        $.when(getModel('/home/posts?sort=' + (params && params.sort ? params.sort : 'newest')))
            .done(function (model) {
                var postCollection = new PostCollection(model.Posts.PagedListItems, { page: model.Query.page, pageSize: model.Query.PageSize, total: model.Posts.TotalResultCount, sortBy: model.Query.Sort });

                if (app.content.currentView instanceof HomePrivateView) {
                    app.content.currentView.showPosts(postCollection);
                } else {
                    var homeView;

                    if (app.authenticatedUser) {
                        var options = { model: app.authenticatedUser.user };
                        if (app.isPrerenderingView('home')) {
                            options['el'] = '.home-private';
                        }
                        homeView = new HomePrivateView(options);
                    } else {
                        homeView = new HomePublicView();
                    }

                    app.showContentView('', homeView, 'home', function () {
                        if (app.authenticatedUser) {
                            homeView.showPosts(postCollection);
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