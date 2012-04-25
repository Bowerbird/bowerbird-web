/// <reference path="../libs/log.js" />
/// <reference path="../libs/jquery-1.7.1.min.js" />
/// <reference path="../libs/underscore.js" />
/// <reference path="../libs/backbone.js" />
/// <reference path="../libs/backbone.marionette.js" />
/// <reference path="namespace.js" />
/// <reference path="app.js" />

// Bowerbird.Views.SidebarCollectionView
// -------------------------------------

// A collection of links in the sidebar
Bowerbird.Views.SidebarCollectionView = (function (app, Bowerbird, Backbone, $, _) {

    var SidebarCollectionView = Backbone.Marionette.CollectionView.extend({
        itemView: Bowerbird.Views.SidebarItemView
    });

    return SidebarCollectionView;

})(Bowerbird.app, Bowerbird, Backbone, jQuery, _);