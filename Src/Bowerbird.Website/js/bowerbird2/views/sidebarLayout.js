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
            defaultMenu: '#default-menu-group',
            projectsMenu: '#project-menu-group',
            watchlistsMenu: '#watch-menu-group'
        }
    });

    return SidebarLayout;

})(Bowerbird.app, Bowerbird, Backbone, jQuery, _);