/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ProjectAboutView
// ----------------

define(['jquery', 'underscore', 'backbone', 'app', 'visualize'], function ($, _, Backbone, app) {

    var ProjectAboutView = Backbone.Marionette.ItemView.extend({
        //className: 'project-about',

        template: 'UserAbout',

        initialize: function (options) {
            _.bindAll(this, 'refresh');
            this.activityTimeseries = options.activityTimeseries;
        },

        serializeData: function () {
            return {
                Model: {
                    User: this.model.toJSON(),
                    UserCountDescription: this.model.get('UserCount') === 1 ? 'Member' : 'Members',
                    SightingCountDescription: this.model.get('SightingCount') === 1 ? 'Sighting' : 'Sightings',
                    PostCountDescription: this.model.get('PostCount') === 1 ? 'Post' : 'Posts',
                    ActivityTimeseries: this.activityTimeseries
                }
            };
        },

        onShow: function () {
            this._showDetails();
            return this;
        },

        showBootstrappedDetails: function () {
            this._showDetails();
        },

        _showDetails: function () {
            this.renderActivityTimeseries();

            var resizeTimer;
            var refresh = this.refresh;
            $(window).resize(function () {
                clearTimeout(resizeTimer);
                resizeTimer = setTimeout(function () {
                    refresh();
                }, 100);
            });
        },

        refresh: function () {
            this.$el.find('#activity-chart #chart-area').empty();
            this.renderActivityTimeseries();
        },

        renderActivityTimeseries: function () {
            this.$el.find('#activity-chart table')
                .visualize({ type: 'line', lineWeight: 2, width: this.$el.find('#chart-area').width() + 'px', height: '180px', colors: ['#308AE3', '#be1e2d', '#ee8310', '#4cba10', '#8d10ee', '#666699', '#5a3b16', '#26a4ed', '#f45a90', '#92d5ea'] })
                .appendTo('#activity-chart #chart-area')
                .trigger('visualizeRefresh');
        },

        showLoading: function () {
            //var that = this;
            //this.$el.find('.stream-message, .stream-load-new, .stream-load-more').remove();
            //this.$el.find('.sighting-items').hide();
            //that.onLoadingStart();
        }
    });

    return ProjectAboutView;

}); 