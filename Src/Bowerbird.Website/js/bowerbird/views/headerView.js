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

        onRender: function () {
            this.$el.find('.new-observation-button').on('click', function (e) {
                e.preventDefault();
                app.observationRouter.navigate($(this).attr('href'), { trigger: true });
                return false;
            });
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
            headerView.render();
        });
    });

    return HeaderView;

});