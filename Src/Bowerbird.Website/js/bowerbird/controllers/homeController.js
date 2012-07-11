/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// HomeController & HomeRouter
// ---------------------------
define(['jquery', 'underscore', 'backbone', 'app', 'views/homepubliclayoutview', 'views/homeprivatelayoutview'],
function ($, _, Backbone, app, HomePublicLayoutView, HomePrivateLayoutView) {
    var HomeRouter = Backbone.Marionette.AppRouter.extend({
        appRoutes: {
            '': 'showHomeStream'
        }
    });

    var HomeController = {};

    // Public API
    // ----------

    HomeController.showHomeStream = function () {
        app.updateTitle('');
        var homeLayoutView = null;

        if (app.authenticatedUser) {
            var homeLayoutView = new HomePrivateLayoutView({ model: app.authenticatedUser.user });
        } else {
            var homeLayoutView = new HomePublicLayoutView();
        }

        app.content[app.getShowViewMethodName('home')](homeLayoutView);

        if (app.isPrerendering('home')) {
            homeLayoutView.showBootstrappedDetails();
        }

        if (app.authenticatedUser) {
            homeLayoutView.showStream();
        }

        app.setPrerenderComplete();
    };

    app.addInitializer(function () {
        this.homeRouter = new HomeRouter({
            controller: HomeController
        });
    });

    return HomeController;

});