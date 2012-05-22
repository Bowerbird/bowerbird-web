/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// HomeController
// --------------

define(['jquery', 'underscore', 'backbone', 'app', 'views/homelayoutview'], function ($, _, Backbone, app, HomeLayoutView) {

    var HomeController = {};

    // Public API
    // ----------

    HomeController.showHomeStream = function (id) {
        var homeLayoutView = new HomeLayoutView({ model: app.user });

        app.content[app.getShowViewMethodName('home')](homeLayoutView);

        if (app.isPrerendering('home')) {
            homeLayoutView.showBootstrappedDetails();
        }

        homeLayoutView.showStream();

        app.setPrerenderComplete();
    };

    return HomeController;

});
