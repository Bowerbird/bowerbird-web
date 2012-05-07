/// <reference path="../libs/log.js" />
/// <reference path="../libs/require/require.js" />
/// <reference path="../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../libs/underscore/underscore.js" />
/// <reference path="../libs/backbone/backbone.js" />
/// <reference path="../libs/backbone.marionette/backbone.marionette.js" />

// Bowerbird.app
// -------------

// Initialises the app, but does not start rendering. That is done 
// when app.start() is called
define(['jquery', 'underscore', 'backbone', 'bootstrap-data', 'models/user', 'marionette'], function ($, _, Backbone, bootstrapData, User) {

    // Create an instance of the app
    var app = new Backbone.Marionette.Application();

    // Let's pollute the global namespace, just a little, for debug purposes :)
    window.Bowerbird = window.Bowerbird || {};
    window.Bowerbird.version = '1.0.0';
    window.Bowerbird.app = app;

    app.addRegions({
        header: 'header',
        footer: 'footer',
        sidebar: '#sidebar',
        content: '#content',
        notifications: '#notifications',
        onlineUsers: '#online-users'
    });

    // Load the bootstrapped data into place
    app.bind('initialize:before', function () {
        // Add the authenticated user to the app for future reference
        if (bootstrapData.AuthenticatedUser) {
            app.authenticatedUser = new User(bootstrapData.AuthenticatedUser);
        }

        // Add the prerendered view string to the app for use by controller duing init of first view
        app.prerenderedView = {
            name: bootstrapData.PrerenderedView,
            isBound: false, // Flag used to determine if prerenderd view has been bound to the object/DOM model
            data: bootstrapData.Model
        };
    });

    // Only start history once app is fully initialised
    app.bind('initialize:after', function () {
        if (Backbone.history) {
            // Start URL and history routing
            Backbone.history.start({ pushState: true });
        }
    });

    // On DOM ready tasks
    $(function () {
        // Start the app as soon as the DOM is ready, loading in the bootstrapped data
        app.start(bootstrapData);
        
        // Register overriding all anchors to channel through router
        $('a').on('click', function (e) {
            e.preventDefault();
            app.groupUserRouter.navigate($(this).attr('href'), true);
            app.contributionRouter.navigate($(this).attr('href'), true);
            return false;
        });

        // Register closing of all popup menus in entire page
        $("body").click(function () {
            $('.sub-menu-button').removeClass('active'); // Make sure to add any new menu button types to the selector
        });
    });

    return app;

});
