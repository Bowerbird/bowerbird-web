/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// HomePrivateView
// ---------------

// The home page view when logged in
define(['jquery', 'underscore', 'backbone', 'app', 'views/activitylistview', 'views/sightinglistview', 'collections/activitycollection', 'collections/sightingcollection'], function ($, _, Backbone, app, ActivityListView, SightingListView, ActivityCollection, SightingCollection) {

    var HomePrivateView = Backbone.Marionette.Layout.extend({
        viewType: 'detail',

        className: 'home-private double',

        template: 'HomePrivate',

        regions: {
            summary: '.summary',
            details: '.details'
        },

        events: {
            'click .activities-tab-button': 'showActivityTabSelection',
            'click .sightings-tab-button': 'showSightingsTabSelection',
            'click .posts-tab-button': 'showPostsTabSelection'
        },

        serializeData: function () {
            return {
                Model: {
                    User: this.model.toJSON(),
                    ShowWelcome: _.contains(app.authenticatedUser.callsToAction, 'welcome'),
                    ShowActivities: this.activeTab === 'activities',
                    ShowSightings: this.activeTab === 'sightings',
                    ShowPosts: this.activeTab === 'posts'
                }
            };
        },

        activeTab: '',

        onShow: function () {
            this.showDetails();
        },

        showBootstrappedDetails: function () {
            this.initializeRegions();
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
                app.vent.trigger('close-call-to-action', 'welcome');
                return false;
            });
        },

        showActivityTabSelection: function (e) {
            e.preventDefault();
            this.switchTabHighlight('activities');
            Backbone.history.navigate($(e.currentTarget).attr('href'), { trigger: true });
        },

        showSightingsTabSelection: function (e) {
            e.preventDefault();
            this.switchTabHighlight('sightings');
            Backbone.history.navigate($(e.currentTarget).attr('href'), { trigger: true });
        },

        showPostsTabSelection: function (e) {
            e.preventDefault();
            this.switchTabHighlight('posts');
            Backbone.history.navigate($(e.currentTarget).attr('href'), { trigger: true });
        },

        showActivity: function () {
            this.switchTabHighlight('activities');

            var activityCollection = new ActivityCollection();
            var options = {
                model: app.authenticatedUser.user,
                collection: activityCollection,
                isHomeStream: true
            };

            if (app.isPrerendering('home')) {
                options['el'] = '.stream';
            }

            var activityListView = new ActivityListView(options);

            if (app.isPrerendering('home')) {
                this.details.attachView(activityListView);
                activityListView.showBootstrappedDetails();
            } else {
                this.details.show(activityListView);
            }

            activityCollection.fetchFirstPage();
        },

        showSightings: function (sightings) {
            this.switchTabHighlight('sightings');

            var sightingCollection = new SightingCollection();
            var options = {
                model: app.authenticatedUser.user,
                collection: sightingCollection
            };

            if (app.isPrerendering('home')) {
                options['el'] = '.sightings';
            }

            var sightingListView = new SightingListView(options);

            if (app.isPrerendering('home')) {
                this.details.attachView(sightingListView);
                sightingListView.showBootstrappedDetails();
            } else {
                this.details.show(sightingListView);
            }

            sightingCollection.reset(sightings.PagedListItems);
        },

        switchTabHighlight: function (tab) {
            this.activeTab = tab;
            this.$el.find('.tab-button').removeClass('selected');
            this.$el.find('.' + tab + '-tab-button').addClass('selected');
        }
    });

    return HomePrivateView;

}); 