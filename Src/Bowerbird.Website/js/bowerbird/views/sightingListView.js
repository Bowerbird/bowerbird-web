// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// SightingListView
// ----------------

// Shows sighting items for selected user/group
define(['jquery', 'underscore', 'backbone', 'app', 'ich', 'views/sightingdetailsview', 'date'],
function ($, _, Backbone, app, ich, SightingDetailsView) {

    var SightingListView = Backbone.Marionette.CompositeView.extend({
        template: 'SightingList',

        itemView: SightingDetailsView,

        activeTab: '',

        events: {
            'click #stream-load-more-button': 'onLoadMoreClicked',
            'click .details-view-button': 'onDetailsTabClicked',
            'click .thumbnails-view-button': 'onThumbnailsTabClicked',
            'click .map-view-button': 'onMapTabClicked',
            'click .sort-button': 'showSortMenu',
            'click .sort-button a': 'changeSort'
        },

        initialize: function (options) {
            _.bindAll(this, 'appendHtml');

            if (!options.activeTab) {
                this.activeTab = 'thumbnails';
            } else {
                this.activeTab = options.activeTab;
            }

            this.activeTab = this.collection.viewType;

            this.newItemsCount = 0;

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
                        View: this.collection.viewType,
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
            var els = this.$el.find('.sighting-item');

            var template = this.collection.viewType == 'details' ? 'SightingListItem' : null;

            _.each(this.collection.models, function (item, index) {
                var childView = new SightingDetailsView({ model: item, tagName: 'li', template: template });
                childView.$el = $(els[index]);
                childView.showBootstrappedDetails();
                childView.delegateEvents();
                this.storeChild(childView);
            }, this);
            this._showDetails();
            this.refresh();
        },

        _showDetails: function () {
            this.$el.find('.tabs li a, .tabs .tab-list-button').not('.map-view-button').tipsy({ gravity: 's' });
            this.$el.find('.map-view-button').tipsy({ gravity: 'se' });

            this.$el.find('h3 a').on('click', function (e) {
                e.preventDefault();
                Backbone.history.navigate($(this).attr('href'), { trigger: true });
                return false;
            });

            this.onLoadingComplete(this.collection);
            this.changeSortLabel(this.collection.sortByType);
            this.switchTabHighlight(this.collection.viewType);
        },

        changeSortLabel: function (value) {
            var label = '';
            switch (value) {
                case 'oldestadded':
                    label = 'Oldest Added';
                    break;
                case 'a-z':
                    label = 'Alphabetical (A-Z)';
                    break;
                case 'z-a':
                    label = 'Alphabetical (Z-A)';
                    break;
                case 'latestadded':
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
            var template = 'SightingTileDetails';
            var className = ' tile-sighting-details';
            if (this.collection.viewType === 'details') {
                template = 'SightingListItem';
                className = '';
            }

            var view = new ItemView({
                template: template,
                model: item,
                tagName: 'li',
                className: 'sighting-item' + className
            });
            return view;
        },

        appendHtml: function (collectionView, itemView) {
            var items = this.collection.pluck('Id');
            var index = _.indexOf(items, itemView.model.id);

            var $li = collectionView.$el.find('.sighting-items > li:eq(' + (index) + ')');

            if ($li.length === 0) {
                collectionView.$el.find('.sighting-items').append(itemView.el);
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
            this.$el.find('.sighting-list').append(ich.StreamLoading());
        },

        onLoadingComplete: function (collection) {
            log(collection);
            this.$el.find('.stream-message, .stream-loading').remove();
            if (collection.length === 0) {
                this.$el.find('.sighting-list').append(ich.StreamMessage());
            }
            if (collection.pageInfo().next) {
                this.$el.find('.sighting-list').append(ich.StreamLoadMore());
            }
        },

        onDetailsTabClicked: function (e) {
            e.preventDefault();
            this.clearListAnPrepareShowLoading();
            this.switchTabHighlight('details');
            Backbone.history.navigate($(e.currentTarget).attr('href'), { trigger: true });
        },

        onThumbnailsTabClicked: function (e) {
            e.preventDefault();
            this.clearListAnPrepareShowLoading();
            this.switchTabHighlight('thumbnails');
            Backbone.history.navigate($(e.currentTarget).attr('href'), { trigger: true });
        },

        onMapTabClicked: function (e) {
            e.preventDefault();
            this.clearListAnPrepareShowLoading();
            this.switchTabHighlight('map');
            Backbone.history.navigate($(e.currentTarget).attr('href'), { trigger: true });
        },

        switchTabHighlight: function (tab) {
            this.activeTab = tab;
            this.$el.find('.tab-image-button').removeClass('selected');
            this.$el.find('.' + tab + '-view-button').addClass('selected');
            this.$el.find('.tabs li a, .tabs .tab-list-button').tipsy.revalidate();
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
            Backbone.history.navigate($(e.currentTarget).attr('href'), { trigger: true });
            return false;
        },

        clearListAnPrepareShowLoading: function () {
            this.$el.find('.stream-message, .stream-load-more, .stream-load-new').remove();
            this.$el.find('.sighting-items').empty();
            this.onLoadingStart();
        },

        showLoading: function () {
            var that = this;
            this.$el.find('.stream-message, .stream-load-new, .stream-load-more').fadeOut(100);
            this.$el.find('.sighting-items').fadeOut(100, function () {
                that.onLoadingStart();
            });
        }
    });

    return SightingListView;

});