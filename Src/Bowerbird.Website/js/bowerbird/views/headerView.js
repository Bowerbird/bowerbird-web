/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// HeaderView
// ----------

// The app's header
define(['jquery', 'underscore', 'backbone', 'app'], function ($, _, Backbone, app) {

    var HeaderView = Backbone.Marionette.ItemView.extend({
        el: 'header',
        
        events: {
            'click .sub-menu-button': 'showMenu'
        },

        showBootstrappedDetails: function () {
            this.$el.find('#explore-menu a').on('click', function (e) {
                e.preventDefault();
                Backbone.history.navigate($(this).attr('href'), { trigger: true });
                return false;
            });
        },
        
        showMenu: function (e) {
            $('.sub-menu-button').removeClass('active');
            $(e.currentTarget).addClass('active');
            e.stopPropagation();
        }
    });

    // Initialize the layout and when the layout has been rendered and displayed, 
    // then start the rest of the application
    app.addInitializer(function (options) {
        $(function () {
            // Render the layout and get it on the screen, first
            var headerView = new HeaderView();

            headerView.on('show', function () {
                app.vent.trigger('headerView:rendered');
            });

            app.header.attachView(headerView);
            headerView.showBootstrappedDetails();
        });
    });

    return HeaderView;

});