/// <reference path="../libs/log.js" />
/// <reference path="../libs/jquery-1.7.1.min.js" />
/// <reference path="../libs/underscore.js" />
/// <reference path="../libs/backbone.js" />
/// <reference path="../libs/backbone.marionette.js" />
/// <reference path="namespace.js" />
/// <reference path="app.js" />

// Bowerbird.Views.SidebarCollectionView
// --------------------------

// The app's header
Bowerbird.Views.SidebarCollectionView = (function (app, Bowerbird, Backbone, $, _) {

    var SidebarCollectionView = Backbone.Marionette.CollectionView.extend({
        el: $('header')
    });

    // Initialize the layout and when the layout has been rendered and displayed, 
    // then start the rest of the application
    app.addInitializer(function () {
        // Render the layout and get it on the screen, first
        var headerView = new HeaderView();

        headerView.on('show', function () {
            Bowerbird.app.vent.trigger('headerView:rendered');
        });

        Bowerbird.app.header.attachView(headerView);
        headerView.render();
    });

    return HeaderView;

})(Bowerbird.app, Bowerbird, Backbone, jQuery, _);