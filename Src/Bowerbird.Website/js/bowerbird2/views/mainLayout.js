/// <reference path="../libs/log.js" />
/// <reference path="../libs/jquery-1.7.1.min.js" />
/// <reference path="../libs/underscore.js" />
/// <reference path="../libs/backbone.js" />
/// <reference path="../libs/backbone.marionette.js" />
/// <reference path="namespace.js" />
/// <reference path="app.js" />

// Bowerbird.Views.MainLayout
// --------------------------

// The section between the header and footer. Contains the sidebar, content and notifications
// regions. Note that the sidebar and notifications are only rendered if the user is authenticated.
Bowerbird.Views.MainLayout = (function (app, Bowerbird, Backbone, $, _) {

    var MainLayout = Backbone.Marionette.Layout.extend({
        el: $('article'),

        regions: {
            sidebar: '#sidebar',
            content: '#content',
            notifications: '#notifications'
        }
    });

    // Initialize the layout and when the layout has been rendered and displayed, 
    // then start the rest of the application
    app.addInitializer(function (options) {
        // Render the layout and get it on the screen, first
        var mainLayout = new MainLayout();

        mainLayout.on('show', function () {
            Bowerbird.app.vent.trigger('mainLayout:rendered');
        });

        Bowerbird.app.main.attachView(mainLayout);
        mainLayout.render(); // Marionette doesn't create the region objects if we don't call render. But calling render calls the template logic. :/ Need to investigate further.

        // Render content based on pre-rendered view
        

        // Only show sidebar and notifications if user is authenticated
        if (options.authenticatedUser) {
            var sidebarLayout = new Bowerbird.Views.SidebarLayout(options);

            sidebarLayout.on('show', function () {
                Bowerbird.app.vent.trigger('sidebarLayout:rendered');
            });

            mainLayout.sidebar.show(sidebarLayout);
            $(mainLayout.el).prepend(sidebarLayout.el);
        }
    });

    return MainLayout;

})(Bowerbird.app, Bowerbird, Backbone, jQuery, _);
