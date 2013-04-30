/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// UserDetailsView
// ---------------

define(['jquery', 'underscore', 'backbone', 'app', 'views/activitylistview', 'views/sightinglistview', 'views/useraboutview', 'views/sightingsearchpanelview'],
function ($, _, Backbone, app, ActivityListView, SightingListView, UserAboutView, SightingSearchPanelView) {

    var UserDetailsView = Backbone.Marionette.Layout.extend({
        viewType: 'detail',

        className: 'user double',

        template: 'User',

        regions: {
            summary: '.summary',
            search: '.search',
            list: '.list'
        },

        events: {
            'click .activities-tab-button': 'showActivityTabSelection',
            'click .sightings-tab-button': 'showSightingsTabSelection',
            'click .about-tab-button': 'showAboutTabSelection'
        },

        activeTab: '',

        initialize: function () {
            _.bindAll(this, 'toggleSearchPanel');
        },

        serializeData: function () {
            return {
                Model: {
                    User: this.model.toJSON(),
                    //IsMember: _.any(app.authenticatedUser.memberships, function (membership) { return membership.GroupId === this.model.id; }, this),
                    SightingCountDescription: this.model.get('SightingCount') === 1 ? 'Sighting' : 'Sightings',
                    ProjectCountDescription: this.model.get('ProjectCount') === 1 ? 'Project' : 'Projects',
                    OrganisationCountDescription: this.model.get('OrganisationCount') === 1 ? 'Organisation' : 'Organisations'
                }
            };
        },

        onShow: function () {
            this._showDetails();
        },

        showBootstrappedDetails: function () {
            this.initializeRegions();
            this._showDetails();
        },

        _showDetails: function () {
        },

        showActivityTabSelection: function (e) {
            e.preventDefault();
            this.showTabSelection('activities', $(e.currentTarget).attr('href'));
        },

        showSightingsTabSelection: function (e) {
            e.preventDefault();
            this.showTabSelection('sightings', $(e.currentTarget).attr('href'));
        },

        showAboutTabSelection: function (e) {
            e.preventDefault();
            this.showTabSelection('about', $(e.currentTarget).attr('href'));
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
                collection: activityCollection
            };

            if (app.isPrerenderingView('users')) {
                options['el'] = '.list > div';
            }

            var activityListView = new ActivityListView(options);

            if (app.isPrerenderingView('users')) {
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

            if (app.isPrerenderingView('users')) {
                options['el'] = '.list > div';
                searchOptions['el'] = '.search > div';
            }

            var sightingListView = new SightingListView(options);
            var sightingSearchPanelView = new SightingSearchPanelView(searchOptions);

            if (app.isPrerenderingView('users')) {
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

        showAbout: function (activityTimeseries) {
            this.switchTabHighlight('about');

            var options = {
                model: this.model
            };

            if (app.isPrerenderingView('users')) {
                options['el'] = '.list > div';
            }

            options.activityTimeseries = activityTimeseries;

            var userAboutView = new UserAboutView(options);

            if (app.isPrerenderingView('users')) {
                this.list.attachView(userAboutView);
                userAboutView.showBootstrappedDetails();
            } else {
                this.list.show(userAboutView);
            }
        },

        switchTabHighlight: function (tab) {
            this.activeTab = tab;
            this.$el.find('.tab-button').removeClass('selected');
            this.$el.find('.' + tab + '-tab-button').addClass('selected');

            if ((tab === 'activities' || tab === 'about') && this.search.currentView) {
                this.search.currentView.$el.hide();
            }
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

    return UserDetailsView;

}); 