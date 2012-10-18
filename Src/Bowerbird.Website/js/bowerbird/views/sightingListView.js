/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// SightingListView
// ----------------

// Shows sighting items for selected user/group
define(['jquery', 'underscore', 'backbone', 'app', 'ich', 'views/sightingitemview', 'date'],
function ($, _, Backbone, app, ich, SightingItemView) {

    var SightingListView = Backbone.Marionette.CompositeView.extend({
        template: 'SightingList',

        itemView: SightingItemView,

        className: 'sightings',

        events: {
            'click #load-more-button': 'onLoadMoreClicked',
            'click #load-new-button': 'onLoadNewClicked'
        },

        initialize: function (options) {
            _.bindAll(this, 'onNewItemReceived', 'appendHtml');

            this.newItemsCount = 0;

            this.collection.on('fetching', this.onLoadingStart, this);
            this.collection.on('fetched', this.onLoadingComplete, this);

            this.newStreamItemsCache = [];

            //app.vent.on('newactivity:observationadded newactivity:postadded newactivity:observationnoteadded', this.onNewStreamItemReceived);
        },

        onShow: function () {
            this.$el = $('.details .sightings');
        },

        showBootstrappedDetails: function () {
        },

        appendHtml: function (collectionView, itemView) {
            var items = this.collection.pluck('Id');
            var index = _.indexOf(items, itemView.model.id);

            var $li = collectionView.$el.find('.sighting-list > li:eq(' + (index) + ')');

            if ($li.length === 0) {
                collectionView.$el.find('.sighting-list').append(itemView.el);
            } else {
                $li.before(itemView.el);
            }

            itemView.start();
        },

        onLoadMoreClicked: function () {
            this.$el.find('.stream-load-more').remove();
            this.collection.fetchNextPage();
        },

        onLoadNewClicked: function () {
            this.$el.find('.stream-load-new').remove();
            this.collection.add(this.newStreamItemsCache);
            this.newStreamItemsCache = [];
            this.newItemsCount = 0;
        },

        onLoadingStart: function (collection) {
            this.$el.append(ich.StreamMessage({ ShowLoader: true }));
        },

        onLoadingComplete: function (collection) {
            this.$el.find('.stream-message').remove();
            if (collection.length === 0) {
                this.$el.append(ich.StreamMessage({ Text: 'No sightings yet! Start now by adding an observation or record.', ShowLoader: false }));
            }
            if (collection.pageInfo().next) {
                this.$el.append(ich.StreamLoadMore());
            }
        },

        onNewItemReceived: function (sightingItem) {
            if (_.any(this.newStreamItemsCache, function (item) { return item.id === sightingItem.id; }, this)) {
                return;
            }
            this.$el.find('.stream-message').remove();
            var streamItemCreatedDateTime = Date.parseExact(sightingItem.get('CreatedDateTime'), 'yyyy-MM-ddTHH:mm:ssZ');

            //log('streamItemCreatedDateTime', streamItemCreatedDateTime);
            //log('baselineDateTime', this.collection.baselineDateTime);

            // Only show a new items message if the item is newer than what we have already
            if (streamItemCreatedDateTime.isAfter(this.collection.baselineDateTime)) {
                log('is after!');
                this.newItemsCount++;
                this.newStreamItemsCache.push(sightingItem);
            }

            if (this.newItemsCount > 0) {
                // Show load new items button
                if (this.$el.find('.stream-load-new').length === 0) {
                    this.$el.prepend(ich.StreamLoadNew());
                } else {
                    this.$el.find('#stream-load-new-button').val('Load ' + this.newItemsCount + ' New Items');
                }
            }
        }
    });

    return SightingListView;

});