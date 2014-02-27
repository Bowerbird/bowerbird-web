/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// HomePrivateView
// ---------------

// The home page view when logged in
define(['jquery', 'underscore', 'backbone', 'app', 'views/activitylistview', 'views/sightinglistview', 'views/postlistview', 'views/sightingsearchpanelview', 'views/postsearchpanelview'],
function ($, _, Backbone, app, ActivityListView, SightingListView, PostListView, SightingSearchPanelView, PostSearchPanelView) {

    var HomePrivateView = Backbone.Marionette.Layout.extend({
        viewType: 'detail',

        className: 'home-private double',

        template: 'HomePrivateIndex',

        regions: {
            summary: '.summary',
            search: '.search',
            list: '.list'
        },

        events: {
            'click .activities-tab-button': 'showActivityTabSelection',
            'click .sightings-tab-button': 'showSightingsTabSelection',
            'click .posts-tab-button': 'showPostsTabSelection'
        },

        initialize: function () {
            _.bindAll(this, 'toggleSearchPanel');
        },

        serializeData: function () {
            return {
                Model: {
                    User: this.model.toJSON(),
                    ShowUserWelcome: _.contains(app.authenticatedUser.callsToAction, 'user-welcome'),
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
            this.$el.find('.close-call-to-action').on('click', function (e) {
                e.preventDefault();
                that.$el.find('.call-to-action').slideUp('fast', function () {
                    that.$el.find('.call-to-action').remove();
                });
                app.vent.trigger('close-call-to-action', 'user-welcome');
                return false;
            });
        },

        showActivityTabSelection: function (e) {
            e.preventDefault();
            this.showTabSelection('activities', $(e.currentTarget).attr('href'));
        },

        showSightingsTabSelection: function (e) {
            e.preventDefault();
            this.showTabSelection('sightings', $(e.currentTarget).attr('href'));
        },

        showPostsTabSelection: function (e) {
            e.preventDefault();
            this.showTabSelection('posts', $(e.currentTarget).attr('href'));
        },

        showTabSelection: function (tab, url) {
            if (this.activeTab === tab) {
                return;
            }

            this.switchTabHighlight(tab);
            this.list.currentView.showLoading();
            Backbone.history.navigate(url, { trigger: true });
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

        showSightings: function (sightingCollection, categorySelectList, fieldSelectList) {
            this.switchTabHighlight('sightings');

            var options = {
                model: this.model,
                collection: sightingCollection
            };

            var searchOptions = {
                sightingCollection: sightingCollection,
                categorySelectList: categorySelectList,
                fieldSelectList: fieldSelectList
            };

            if (app.isPrerenderingView('home')) {
                options['el'] = '.list > div';
                searchOptions['el'] = '.search > div';
            }

            var sightingListView = new SightingListView(options);
            var sightingSearchPanelView = new SightingSearchPanelView(searchOptions);

            if (app.isPrerenderingView('home')) {
                this.list.attachView(sightingListView);
                sightingListView.showBootstrappedDetails();

                this.search.attachView(sightingSearchPanelView);
                sightingSearchPanelView.showBootstrappedDetails();
            } else {
                this.list.show(sightingListView);

                this.search.show(sightingSearchPanelView);
            }

            sightingListView.on('toggle-search', this.toggleSearchPanel);

            if (sightingCollection.hasSearchCriteria()) {
                this.$el.find('.search-bar').slideDown();
            }
        },

        showPosts: function (postCollection, fieldSelectList) {
            this.switchTabHighlight('posts');

            var options = {
                model: this.model,
                collection: postCollection
            };

            var searchOptions = {
                postCollection: postCollection,
                fieldSelectList: fieldSelectList
            };

            if (app.isPrerenderingView('home')) {
                options['el'] = '.list > div';
                searchOptions['el'] = '.search > div';
            }

            var postListView = new PostListView(options);
            var postSearchPanelView = new PostSearchPanelView(searchOptions);

            if (app.isPrerenderingView('home')) {
                this.list.attachView(postListView);
                postListView.showBootstrappedDetails();

                this.search.attachView(postSearchPanelView);
                postSearchPanelView.showBootstrappedDetails();
            } else {
                this.list.show(postListView);

                this.search.show(postSearchPanelView);
            }

            postListView.on('toggle-search', this.toggleSearchPanel);

            if (postCollection.hasSearchCriteria()) {
                this.$el.find('.search-bar').slideDown();
            }
        },

        switchTabHighlight: function (tab) {
            this.activeTab = tab;
            this.$el.find('.tab-button').removeClass('selected');
            this.$el.find('.' + tab + '-tab-button').addClass('selected');

//            if (tab === 'activities' && this.search.currentView) {
//                this.search.currentView.$el.hide();
//            }
        },

        toggleSearchPanel: function () {
            log('search bar', this.$el.find('.search-bar'));
            if (this.$el.find('.search-bar').is(':visible')) {
                log('search is visible');
                this.$el.find('.search-bar').slideToggle();
            } else {
                log('search is not visible');
                var that = this;
                this.$el.find('.search-bar').slideToggle(function () {
                    that.$el.find('.search-bar #query').focus();
                });
            }
        }
    });

    return HomePrivateView;

}); 