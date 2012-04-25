/// <reference path="../libs/log.js" />
/// <reference path="../libs/jquery-1.7.1.min.js" />
/// <reference path="../libs/underscore.js" />
/// <reference path="../libs/backbone.js" />
/// <reference path="../libs/backbone.marionette.js" />

// Bowerbird.app
// -------------

// Initialises the app, but does not start rendering. That is done 
// when Bowerbird.app.start() is called
Bowerbird.app = (function (Backbone) {

    // Create an instance of the app
    var app = new Backbone.Marionette.Application();

    app.addRegions({
        header: 'header',
        sidebar: 'sidebar',
        content: 'content',
        notifications: 'notifications',
        footer: 'footer',
        onlineUsers: '#online-users'
    });

    // Only start history once app is fully initialised
    app.bind('initialize:after', function () {
        if (Backbone.history) {
            // Start URL and history routing
            Backbone.history.start({ pushState: true });
        }
    });

    return app;

})(Backbone);