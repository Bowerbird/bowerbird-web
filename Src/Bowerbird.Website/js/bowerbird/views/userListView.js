// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// UserListView
// ------------

// Shows users for selected user/group
define(['jquery', 'underscore', 'backbone', 'app', 'ich', 'views/userdetailsview', 'date', 'tipsy'],
function ($, _, Backbone, app, ich, UserDetailsView) {

    var UserListView = Backbone.Marionette.CompositeView.extend({
        template: 'UserList',

        itemView: UserDetailsView,

        activeTab: '',

        events: {
            'click #stream-load-more-button': 'onLoadMoreClicked',
            'click .sort-button': 'showSortMenu',
            'click .sort-button a': 'changeSort'
        },

        initialize: function (options) {
            _.bindAll(this, 'appendHtml');

            this.collection.on('fetching', this.onLoadingStart, this);
            this.collection.on('fetched', this.onLoadingComplete, this);
            this.collection.on('reset', this.onLoadingComplete, this);
        },

        serializeData: function () {
            return {
                Model: {
                    Query: {
                        Id: this.collection.projectId,
                        Page: this.collection.pageSize,
                        PageSize: this.collection.page,
                        Sort: this.collection.sortByType
                    }
                }
            };
        },

        onShow: function () {
            this.refresh();
        },

        onRender: function () {
            this._showDetails();
            this.refresh();
        },

        showBootstrappedDetails: function () {
            var els = this.$el.find('.user-item');
            _.each(this.collection.models, function (item, index) {
                var childView = new UserDetailsView({ model: item, tagName: 'li' });
                childView.$el = $(els[index]);
                childView.showBootstrappedDetails();
                childView.delegateEvents();
                this.storeChild(childView);
            }, this);
            this._showDetails();
        },

        _showDetails: function () {
            this.$el.find('.tabs li a, .tabs .tab-list-button').tipsy({ gravity: 's' });

            this.$el.find('h3 a').on('click', function (e) {
                e.preventDefault();
                Backbone.history.navigate($(this).attr('href'), { trigger: true });
                return false;
            });

            this.onLoadingComplete(this.collection);
            this.changeSortLabel(this.collection.sortByType);
        },

        changeSortLabel: function (value) {
            var label = '';
            switch (value) {
                case 'oldest':
                    label = 'Oldest Added';
                    break;
                case 'a-z':
                    label = 'Alphabetical (A-Z)';
                    break;
                case 'z-a':
                    label = 'Alphabetical (Z-A)';
                    break;
                case 'newest':
                default:
                    label = 'Latest Added';
                    break;
            }

            this.$el.find('.sort-button .tab-list-selection').empty().text(label);
        },

        refresh: function () {
            _.each(this.children, function (childView) {
                childView.refresh();
            });
        },

        buildItemView: function (item, ItemView) {
            var view = new ItemView({
                model: item,
                tagName: 'li',
                className: 'user-item tile-user-details'
            });
            return view;
        },

        appendHtml: function (collectionView, itemView) {
            var items = this.collection.pluck('Id');
            var index = _.indexOf(items, itemView.model.id);

            var $li = collectionView.$el.find('.user-items > li:eq(' + (index) + ')');

            if ($li.length === 0) {
                collectionView.$el.find('.user-items').append(itemView.el);
            } else {
                $li.before(itemView.el);
            }

            itemView.refresh();
        },

        onLoadMoreClicked: function () {
            this.$el.find('.stream-message, .stream-load-more').remove();
            this.collection.fetchNextPage();
        },

        onLoadingStart: function (collection) {
            this.$el.find('.user-list').append(ich.StreamLoading());
        },

        onLoadingComplete: function (collection) {
            log(collection);
            this.$el.find('.stream-message, .stream-loading').remove();
            if (collection.length === 0) {
                this.$el.find('.user-list').append(ich.StreamMessage());
            }
            if (collection.pageInfo().next) {
                this.$el.find('.user-list').append(ich.StreamLoadMore());
            }
        },

        showSortMenu: function (e) {
            app.vent.trigger('close-sub-menus');
            $(e.currentTarget).addClass('active');
            e.stopPropagation();
        },

        changeSort: function (e) {
            e.preventDefault();
            this.$el.find('.sort-button .tab-list-selection').empty().text($(e.currentTarget).text());
            app.vent.trigger('close-sub-menus');
            this.$el.find('.tabs li a, .tabs .tab-list-button').tipsy.revalidate();
            this.clearListAnPrepareShowLoading();
            Backbone.history.navigate($(e.currentTarget).attr('href'), { trigger: true });
            return false;
        },

        clearListAnPrepareShowLoading: function () {
            this.$el.find('.stream-message, .stream-load-more, .stream-load-new').remove();
            this.$el.find('.user-items').empty();
            this.onLoadingStart();
        },

        showLoading: function () {
            var that = this;
            this.$el.find('.stream-message, .stream-load-new, .stream-load-more').fadeOut(100);
            this.$el.find('.user-items').fadeOut(100, function () {
                that.onLoadingStart();
            });
        }
    });

    return UserListView;

});