// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// PostListView
// ------------

// Shows post items for selected user/group
define(['jquery', 'underscore', 'backbone', 'app', 'ich', 'views/postdetailsview', 'date'],
function ($, _, Backbone, app, ich, PostDetailsView) {

    var PostListView = Backbone.Marionette.CompositeView.extend({
        template: 'PostList',

        itemView: PostDetailsView,

        activeTab: '',

        events: {
            'click #stream-load-more-button': 'onLoadMoreClicked',
            'click .sort-button': 'showSortMenu',
            'click .sort-button a': 'changeSort',
            'click .search-button': 'showHideSearch'
        },

        initialize: function (options) {
            _.bindAll(this, 'appendHtml', 'clearListAnPrepareShowLoading');

            this.newItemsCount = 0;

            this.collection.on('fetching', this.onLoadingStart, this);
            this.collection.on('fetched', this.onLoadingComplete, this);
            this.collection.on('reset', this.onLoadingComplete, this);
            this.collection.on('search-reset', this.clearListAnPrepareShowLoading, this);
        },

        serializeData: function () {
            return {
                Model: {
                    Query: {
                        Id: this.collection.groupId,
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
            var els = this.$el.find('.post-item');

            _.each(this.collection.models, function (item, index) {
                var childView = new PostDetailsView({ model: item, tagName: 'li', template: 'PostListItem' });
                childView.$el = $(els[index]);
                childView.showBootstrappedDetails();
                childView.delegateEvents();
                this.storeChild(childView);
            }, this);
            this._showDetails();
            this.refresh();
        },

        _showDetails: function () {
            this.$el.find('.tabs li a, .tabs .tab-list-button, .search-button').tipsy({ gravity: 's' });

            this.$el.find('h3 a').on('click', function (e) {
                e.preventDefault();
                Backbone.history.navigate($(this).attr('href'), { trigger: true });
                return false;
            });

            this.onLoadingComplete(this.collection);
            this.changeSortLabel(this.collection.sortByType);

            this.collection.on('criteria-changed', this.clearListAnPrepareShowLoading);
        },

        changeSortLabel: function (value) {
            var label = '';
            switch (value) {
                case 'oldest':
                    label = 'Oldest Added';
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
                template: 'PostListItem',
                model: item,
                tagName: 'li',
                className: 'post-item'
            });
            return view;
        },

        appendHtml: function (collectionView, itemView) {
            var items = this.collection.pluck('Id');
            var index = _.indexOf(items, itemView.model.id);

            var $li = collectionView.$el.find('.post-items > li:eq(' + (index) + ')');

            if ($li.length === 0) {
                collectionView.$el.find('.post-items').append(itemView.el);
            } else {
                $li.before(itemView.el);
            }

            itemView.refresh();
        },

        onLoadMoreClicked: function () {
            this.$el.find('.stream-message, .stream-load-more').remove();
            this.collection.fetchNextPage();
        },

        onLoadNewClicked: function () {
            this.$el.find('.stream-message, .stream-load-new').remove();
            this.collection.add(this.newStreamItemsCache);
            this.newStreamItemsCache = [];
            this.newItemsCount = 0;
        },

        onLoadingStart: function (collection) {
            this.$el.find('.post-list').append(ich.StreamLoading());
        },

        onLoadingComplete: function (collection) {
            this.$el.find('.stream-message, .stream-loading').remove();
            if (collection.length === 0) {
                this.$el.find('.post-list').append(ich.StreamMessage());
            }
            if (collection.pageInfo().next) {
                this.$el.find('.post-list').append(ich.StreamLoadMore());
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
            this.clearListAnPrepareShowLoading();
            this.collection.changeSort($(e.currentTarget).data('sort'));
            Backbone.history.navigate($(e.currentTarget).attr('href'), { trigger: false });
            return false;
        },

        clearListAnPrepareShowLoading: function () {
            this.$el.find('.stream-message, .stream-load-more, .stream-load-new, .stream-loading').remove();
            this.$el.find('.post-items').empty();
        },

        showLoading: function () {
            this.$el.find('.stream-message, .stream-load-new, .stream-load-more').remove();
            this.$el.find('.post-items, .tab-bar-right').hide();
            this.onLoadingStart();
        },

        showHideSearch: function () {
            this.trigger('toggle-search');
        }
    });

    return PostListView;

});