/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// HomePrivateView
// ---------------

// The home page view when logged in
define(['jquery', 'underscore', 'backbone', 'app', 'views/activitylistview', 'views/sightinglistview'], 
function ($, _, Backbone, app, ActivityListView, SightingListView) {

    var HomePrivateView = Backbone.Marionette.Layout.extend({
        viewType: 'detail',

        className: 'home-private double',

        template: 'HomePrivateIndex',

        regions: {
            summary: '.summary',
            list: '.list'
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
                    ShowActivities: this.activeTab === 'activities' || this.activeTab === '',
                    ShowSightings: this.activeTab === 'sightings',
                    ShowPosts: this.activeTab === 'posts'
                }
            };
        },

        activeTab: '',

        onShow: function () {
            this._showDetails();
        },

        showBootstrappedDetails: function () {
            this.initializeRegions();
            this._showDetails();
        },

        _showDetails: function () {
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
            this.list.currentView.showLoading();
            Backbone.history.navigate($(e.currentTarget).attr('href'), { trigger: true });
        },

        showSightingsTabSelection: function (e) {
            e.preventDefault();
            this.switchTabHighlight('sightings');
            this.list.currentView.showLoading();
            Backbone.history.navigate($(e.currentTarget).attr('href'), { trigger: true });
        },

        showPostsTabSelection: function (e) {
            e.preventDefault();
            this.switchTabHighlight('posts');
            this.list.currentView.showLoading();
            Backbone.history.navigate($(e.currentTarget).attr('href'), { trigger: true });
        },

        showActivity: function (activityCollection) {
            this.switchTabHighlight('activities');

            var options = {
                model: this.model,
                collection: activityCollection,
                isHomeStream: true
            };

            if (app.isPrerenderingView('home')) {
                options['el'] = '.list > div';
            }

            var activityListView = new ActivityListView(options);

            if (app.isPrerenderingView('home')) {
                this.list.attachView(activityListView);
                activityListView.showBootstrappedDetails();
            } else {
                this.list.show(activityListView);
            }
        },

        showSightings: function (sightingCollection) {
            this.switchTabHighlight('sightings');

            var options = {
                model: this.model,
                collection: sightingCollection
            };

            if (app.isPrerenderingView('home')) {
                options['el'] = '.list > div';
            }

            var sightingListView = new SightingListView(options);

            if (app.isPrerenderingView('home')) {
                this.list.attachView(sightingListView);
                sightingListView.showBootstrappedDetails();
            } else {
                this.list.show(sightingListView);
            }
        },

        switchTabHighlight: function (tab) {
            this.activeTab = tab;
            this.$el.find('.tab-button').removeClass('selected');
            this.$el.find('.' + tab + '-tab-button').addClass('selected');
        }
    });

    return HomePrivateView;

}); 