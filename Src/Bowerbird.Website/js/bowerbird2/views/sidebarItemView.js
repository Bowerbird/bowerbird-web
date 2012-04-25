/// <reference path="../libs/log.js" />
/// <reference path="../libs/jquery-1.7.1.min.js" />
/// <reference path="../libs/underscore.js" />
/// <reference path="../libs/backbone.js" />
/// <reference path="../libs/backbone.marionette.js" />
/// <reference path="namespace.js" />
/// <reference path="app.js" />

// Bowerbird.Views.SidebarItemView
// -------------------------------------

// An item in the sidebar
Bowerbird.Views.SidebarItemView = (function (app, Bowerbird, Backbone, $, _) {

    var SidebarItemView = Backbone.Marionette.ItemView.extend({
    });

    return SidebarItemView;

})(Bowerbird.app, Bowerbird, Backbone, jQuery, _);
