/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// UserExploreView
// ---------------

define(['jquery', 'underscore', 'backbone', 'app', 'views/userlistview', 'views/usersearchpanelview'],
function ($, _, Backbone, app, UserListView, UserSearchPanelView) {

    var UserExploreView = Backbone.Marionette.Layout.extend({
        viewType: 'details',

        className: 'users double',

        template: 'UserExplore',

        regions: {
            search: '.search',
            list: '.list'
        },

        initialize: function (options) {
            _.bindAll(this, 'toggleSearchPanel');

            this.userCollection = options.userCollection;
            this.fieldSelectList = options.fieldSelectList;
        },

        serializeData: function () {
            return {
                Model: {
                }
            };
        },

        onShow: function () {
            var options = {
                collection: this.userCollection
            };

            var searchOptions = {
                userCollection: this.userCollection,
                fieldSelectList: this.fieldSelectList
            };

            if (app.isPrerenderingView('users')) {
                options['el'] = '.list > div';
                searchOptions['el'] = '.search > div';
            }

            var userListView = new UserListView(options);
            var userSearchPanelView = new UserSearchPanelView(searchOptions);

            if (app.isPrerenderingView('users')) {
                this.list.attachView(userListView);
                userListView.showBootstrappedDetails();

                this.search.attachView(userSearchPanelView);
                userSearchPanelView.showBootstrappedDetails();
            } else {
                this.list.show(userListView);

                this.search.show(userSearchPanelView);
            }

            this._showDetails();
        },

        showBootstrappedDetails: function () {
            this.initializeRegions();

            var options = {
                collection: this.userCollection
            };

            var searchOptions = {
                userCollection: this.userCollection,
                fieldSelectList: this.fieldSelectList
            };

            if (app.isPrerenderingView('users')) {
                options['el'] = '.list > div';
                searchOptions['el'] = '.search > div';
            }

            var userListView = new UserListView(options);
            var userSearchPanelView = new UserSearchPanelView(searchOptions);

            if (app.isPrerenderingView('users')) {
                this.list.attachView(userListView);
                userListView.showBootstrappedDetails();

                this.search.attachView(userSearchPanelView);
                userSearchPanelView.showBootstrappedDetails();
            } else {
                this.list.show(userListView);

                this.search.show(userSearchPanelView);
            }

            this._showDetails();
        },

        _showDetails: function () {
            var that = this;

            this.list.currentView.on('toggle-search', this.toggleSearchPanel);

            if (this.userCollection.hasSearchCriteria()) {
                this.$el.find('.search-bar').slideDown();
            }

            this.$el.find('.close-call-to-action').on('click', function (e) {
                e.preventDefault();
                that.$el.find('.call-to-action').slideUp('fast', function () {
                    that.$el.find('.call-to-action').remove();
                });
                app.vent.trigger('close-call-to-action', 'user-welcome');
                return false;
            });
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

    return UserExploreView;

}); 