/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ActivityListView
// ----------------

// Shows stream items for selected user/group
define(['jquery', 'underscore', 'backbone', 'app', 'ich', 'views/activityitemview', 'date', 'tipsy'],
function ($, _, Backbone, app, ich, ActivityItemView) {

    var ActivityListView = Backbone.Marionette.CompositeView.extend({
        template: 'ActivityList',

        itemView: ActivityItemView,

        events: {
            'click #stream-load-more-button': 'onLoadMoreClicked',
            'click #stream-load-new-button': 'onLoadNewClicked'
        },

        initialize: function (options) {
            _.bindAll(this, 'onNewStreamItemReceived', 'appendHtml', 'onLoadingStart');

            this.newItemsCount = 0;
            this.isHomeStream = options.isHomeStream && options.isHomeStream === true ? true : false;

            this.collection.on('fetching', this.onLoadingStart, this);
            this.collection.on('fetched', this.onLoadingComplete, this);

            this.newStreamItemsCache = [];

            app.vent.on('newactivity:sightingadded newactivity:postadded newactivity:sightingnoteadded', this.onNewStreamItemReceived);
        },

        onShow: function () {
            this.refresh();
        },

        onRender: function () {
            this._showDetails();
            this.refresh();
        },

        showBootstrappedDetails: function () {
            var els = this.$el.find('.activity-item');
            _.each(this.collection.models, function (item, index) {
                var childView = new ActivityItemView({ model: item });
                childView.setElement($(els[index]));
                childView.showBootstrappedDetails();
                this.storeChild(childView);
            }, this);
            this._showDetails();
            this.refresh();
        },

        _showDetails: function () {
            this.$el.find('.tabs li a, .tabs .tab-list-button').not('.list-view-button').tipsy({ gravity: 's' });
            this.$el.find('.list-view-button').tipsy({ gravity: 'se' });

            this.onLoadingComplete(this.collection);
        },

        refresh: function () {
            _.each(this.children, function (childView) {
                childView.refresh();
            });
        },

        appendHtml: function (collectionView, itemView) {
            var items = this.collection.pluck('Id');
            var index = _.indexOf(items, itemView.model.id);

            var $li = collectionView.$el.find('.activity-items > li:eq(' + (index) + ')');

            if (itemView.model.get('Type') == "sightingadded") {
                itemView.$el.addClass('sightingadded-activity-item');
            } else if (itemView.model.get('Type') == "sightingnoteadded") {
                itemView.$el.addClass('sightingnoteadded-activity-item');
            }

            if ($li.length === 0) {
                collectionView.$el.find('.activity-items').append(itemView.el);
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
            this.$el.find('.activity-list').append(ich.StreamLoading());
        },

        onLoadingComplete: function (collection) {
            this.$el.find('.stream-message, .stream-loading').remove();
            if (collection.length === 0) {
                this.$el.find('.activity-list').append(ich.StreamMessage());
            }
            if (collection.pageInfo().next) {
                this.$el.find('.activity-list').append(ich.StreamLoadMore());
            }
        },

        onNewStreamItemReceived: function (streamItem) {
            if (_.any(this.newStreamItemsCache, function (item) { return item.id === streamItem.id; }, this)) {
                return;
            }
            this.$el.find('.stream-message').remove();
            var streamItemCreatedDateTime = Date.parseExact(streamItem.get('CreatedDateTime'), 'yyyy-MM-ddTHH:mm:ssZ');

            //log('streamItemCreatedDateTime', streamItemCreatedDateTime);
            //log('baselineDateTime', this.collection.baselineDateTime);

            // Only show a new items message if the item is newer than what we have already
            if (streamItemCreatedDateTime.isAfter(this.collection.baselineDateTime)) {
                log('is after!');
                this.newItemsCount++;
                this.newStreamItemsCache.push(streamItem);
            }

            if (this.newItemsCount > 0) {
                // Show load new items button
                if (this.$el.find('.stream-load-new').length === 0) {
                    this.$el.find('.activity-list').prepend(ich.StreamLoadNew());
                } else {
                    this.$el.find('#stream-load-new-button').val('Load ' + this.newItemsCount + ' New Items');
                }
            }
        },

        showLoading: function () {
            var that = this;
            this.$el.find('.stream-message, .stream-load-new, .stream-load-more').fadeOut(100);
            this.$el.find('.activity-items').fadeOut(100, function () {
                that.onLoadingStart();
            });
        }
    });

    return ActivityListView;

});