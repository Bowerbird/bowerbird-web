/// <reference path="../libs/log.js" />
/// <reference path="../libs/jquery-1.7.1.min.js" />
/// <reference path="../libs/underscore.js" />
/// <reference path="../libs/backbone.js" />
/// <reference path="../libs/backbone.marionette.js" />
/// <reference path="namespace.js" />
/// <reference path="app.js" />

// Bowerbird.Views.SidebarLayout
// --------------------------

// The left hand side bar that is shown to authenticated users.
Bowerbird.Views.SidebarLayout = (function (app, Bowerbird, Backbone, $, _) {

    var SidebarLayout = Backbone.Marionette.Layout.extend({
        tagName: 'section',

        id: 'sidebar',

        className: 'triple-1',

        template: 'Sidebar',

        regions: {
            projectsMenu: '#project-menu-group',
            watchlistsMenu: '#watch-menu-group'
        },

        initialize: function (options) {
            log(options);

            options['el'] = $('#project-menu-group');

            var projectsCollectionView = new Bowerbird.Views.SidebarCollectionView(options);

            projectsCollectionView.on('show', function () {
                Bowerbird.app.vent.trigger('projectsCollectionView:rendered');
            });

            this.projectsMenu.show(projectsCollectionView);
        }
    });

    // Initialize the layout and when the layout has been rendered and displayed, 
    // then start the rest of the application
    app.addInitializer(function (options) {
        // Only show sidebar if user is authenticated
        if (options.authenticatedUser) {
            // Render the layout and get it on the screen, first
            var sidebarLayout = new Bowerbird.Views.SidebarLayout(options);

            sidebarLayout.on('show', function () {
                Bowerbird.app.vent.trigger('sidebarLayout:rendered');
            });

            Bowerbird.app.sidebar.show(sidebarLayout);
            $('article').prepend(sidebarLayout.el);
        }
    });

    return SidebarLayout;

})(Bowerbird.app, Bowerbird, Backbone, jQuery, _);