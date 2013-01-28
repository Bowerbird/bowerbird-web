/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// OrganisationDetailsView
// ------------------

define(['jquery', 'underscore', 'backbone', 'app', 'views/activitylistview', 'views/sightinglistview', 'views/postlistview', 'views/userlistview', 'views/organisationaboutview', 'views/sightingsearchpanelview', 'views/postsearchpanelview'],
function ($, _, Backbone, app, ActivityListView, SightingListView, PostListView, UserListView, OrganisationAboutView, SightingSearchPanelView, PostSearchPanelView) {

    var OrganisationDetailsView = Backbone.Marionette.Layout.extend({
        viewType: 'detail',

        className: 'organisation double',

        template: 'Organisation',

        regions: {
            summary: '.summary',
            search: '.search',
            list: '.list'
        },

        events: {
            'click .activities-tab-button': 'showActivityTabSelection',
            'click .posts-tab-button': 'showPostsTabSelection',
            'click .members-tab-button': 'showMembersTabSelection',
            'click .about-tab-button': 'showAboutTabSelection'
        },

        activeTab: '',

        initialize: function () {
            _.bindAll(this, 'toggleSearchPanel');
        },

        serializeData: function () {
            return {
                Model: {
                    Organisation: this.model.toJSON(),
                    //IsMember: _.any(app.authenticatedUser.memberships, function (membership) { return membership.GroupId === this.model.id; }, this),
                    MemberCountDescription: this.model.get('MemberCount') === 1 ? 'Member' : 'Members',
                    PostCountDescription: this.model.get('PostCount') === 1 ? 'Post' : 'Posts'
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

        showPostsTabSelection: function (e) {
            e.preventDefault();
            this.showTabSelection('posts', $(e.currentTarget).attr('href'));
        },

        showMembersTabSelection: function (e) {
            e.preventDefault();
            this.showTabSelection('members', $(e.currentTarget).attr('href'));
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

            if (app.isPrerenderingView('organisations')) {
                options['el'] = '.list > div';
            }

            var activityListView = new ActivityListView(options);

            if (app.isPrerenderingView('organisations')) {
                this.list.attachView(activityListView);
                activityListView.showBootstrappedDetails();
            } else {
                this.list.show(activityListView);
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

            if (app.isPrerenderingView('organisations')) {
                options['el'] = '.list > div';
                searchOptions['el'] = '.search > div';
            }

            var postListView = new PostListView(options);
            var postSearchPanelView = new PostSearchPanelView(searchOptions);

            if (app.isPrerenderingView('organisations')) {
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

        showMembers: function (userCollection) {
            this.switchTabHighlight('members');

            var options = {
                model: this.model,
                collection: userCollection
            };

            if (app.isPrerenderingView('organisations')) {
                options['el'] = '.list > div';
            }

            var userListView = new UserListView(options);

            if (app.isPrerenderingView('organisations')) {
                this.list.attachView(userListView);
                userListView.showBootstrappedDetails();
            } else {
                this.list.show(userListView);
            }
        },

        showAbout: function (organisationAdministrators, activityTimeseries) {
            this.switchTabHighlight('about');

            var options = {
                model: this.model
            };

            if (app.isPrerenderingView('organisations')) {
                options['el'] = '.list > div';
            }

            options.organisationAdministrators = organisationAdministrators;
            options.activityTimeseries = activityTimeseries;

            var organisationAboutView = new OrganisationAboutView(options);

            if (app.isPrerenderingView('organisations')) {
                this.list.attachView(organisationAboutView);
                organisationAboutView.showBootstrappedDetails();
            } else {
                this.list.show(organisationAboutView);
            }
        },

        switchTabHighlight: function (tab) {
            this.activeTab = tab;
            this.$el.find('.tab-button').removeClass('selected');
            this.$el.find('.' + tab + '-tab-button').addClass('selected');

            if ((tab === 'activities' || tab === 'members' || tab === 'about') && this.search.currentView) {
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

    return OrganisationDetailsView;

}); 