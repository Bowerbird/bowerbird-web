/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// HomePrivateLayoutView
// ---------------------

// The home page view when logged in
define(['jquery', 'underscore', 'backbone', 'app', 'views/streamview', 'collections/activitycollection'], function ($, _, Backbone, app, StreamView, ActivityCollection) {

    var HomePrivateLayoutView = Backbone.Marionette.Layout.extend({
        className: 'home',

        template: 'Home',

        regions: {
            summary: '.summary',
            details: '.details'
        },

        serializeData: function () {
            return {
                Model: {
                    User: this.model.toJSON()
                }
            };
        },

        onShow: function () {
            this.showDetails();
        },

        showBootstrappedDetails: function () {
            this.initializeRegions();
            this.$el = $('#content .home');
            this.showDetails();
        },

        showDetails: function () {
            var that = this;
            this.$el.find('.close-intro').on('click', function (e) {
                e.preventDefault();
                that.$el.find('#intro').slideUp('fast', function () {
                    that.$el.find('#intro').remove();
                });
                // TODO: Save intro closed status
                return false;
            });
        },

        showStream: function () {
            var activityCollection = new ActivityCollection();
            var options = {
                model: app.authenticatedUser.user,
                collection: activityCollection,
                isHomeStream: true
            };

            if (app.isPrerendering('home')) {
                options['el'] = '.stream';
            }

            var streamView = new StreamView(options);

            if (app.isPrerendering('home')) {
                this.details.attachView(streamView);
                streamView.showBootstrappedDetails();
            } else {
                this.details.show(streamView);
            }

            activityCollection.fetchFirstPage();
        }
    });

    return HomePrivateLayoutView;

}); 