/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ActivityListView
// ----------------

// Shows stream items for selected user/group
define(['jquery', 'underscore', 'backbone', 'app', 'ich', 'views/activityitemview', 'moment', 'tipsy'],
function ($, _, Backbone, app, ich, ActivityItemView, moment) {

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

            app.vent.on('newactivity:sightingadded newactivity:postadded newactivity:sightingnoteadded newactivity:identificationadded', this.onNewStreamItemReceived);
        },

        onShow: function () {
            this._showDetails();
            this.refresh();
        },

        onRender: function () {
            //this.refresh();
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

            this.enableInfiniteScroll();

            this.refresh();
        },

        refresh: function () {
            _.each(this.children, function (childView) {
                childView.refresh();
            });
        },

        enableInfiniteScroll: function () {
            // Infinite scroll
            var that = this;
            this.$el.find('.activity-list').infinitescroll({
                navSelector: '.stream-load-more', // selector for the paged navigation (it will be hidden)
                nextSelector: ".next-page", // selector for the NEXT link (to page 2)
                itemSelector: '.activity-item', // selector for all items you'll retrieve                
                dataType: 'json',
                appendCallback: false,
                binder: $(window), // used to cache the selector for the element that will be scrolling
                maxPage: that.collection.total > 0 ? (Math.floor(that.collection.total / that.collection.pageSize)) + 1 : 0, // Total number of navigable pages
                animate: false,
                pixelsFromNavToBottom: 30,
                path: function (currentPage) {
                    //return that.buildPagingUrl(true);
                    return that.collection.searchUrl(true, currentPage);
                },
                loading: {
                    msg: $(ich.StreamLoading()),
                    speed: 1
                }
            }, function (json, opts) {
                // Get current page
                //var page = opts.state.currPage;

                for (var x = 0; x < json.Model.Activities.PagedListItems.length; x++) {
                    that.collection.add(json.Model.Activities.PagedListItems[x]);
                }

                var maxPage = that.collection.total > 0 ? (Math.floor(that.collection.total / that.collection.pageSize)) + 1 : 0;
                if (json.Model.Activities.Page === maxPage) {
                    that.$el.find('.activity-list').append('<div class="no-more-items">You\'ve reached the end. Time to add some more sightings or join more projects!</div>');
                    opts.state.isDone = true;
                }
            });

            this.$el.find('.stream-load-more').hide();
        },

        appendHtml: function (collectionView, itemView) {
            $.when(this.sortAndAppend(collectionView, itemView))
                .then(function () {
                    itemView.refresh();
                });

            //            var items = this.collection.pluck('Id');
            //            var index = _.indexOf(items, itemView.model.id);

            //            var $li = collectionView.$el.find('.activity-items > li:eq(' + (index) + ')');

            //            itemView.$el.addClass(itemView.model.get('Type') + '-activity-item');

            //            if ($li.length === 0) {
            //                collectionView.$el.find('.activity-items').append(itemView.el);
            //            } else {
            //                $li.before(itemView.el);
            //            }

            //            itemView.refresh();
        },

        sortAndAppend: function (collectionView, itemView) {
            var that = this;
            return $.Deferred(function (dfd) {
                var items = that.collection.pluck('Id');
                var index = _.indexOf(items, itemView.model.id);

                var $li = collectionView.$el.find('.activity-items > li:eq(' + (index) + ')');

                itemView.$el.addClass(itemView.model.get('Type') + '-activity-item');

                if ($li.length === 0) {
                    collectionView.$el.find('.activity-items').append(itemView.el);
                } else {
                    $li.before(itemView.el);
                }

                dfd.resolve();
            }).promise();
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
            //            if (collection.pageInfo().next) {
            //                this.$el.find('.activity-list').append(ich.StreamLoadMore());
            //            }


            if (collection.pageInfo().next) {
                var pagingInfo = {
                    NextUrl: this.collection.searchUrl(true, this.collection.page + 1)
                };

                if (this.collection.page > 1) {
                    pagingInfo['PreviousUrl'] = this.collection.searchUrl(true, this.collection.page - 1);
                }

                this.$el.find('.activity-list').append(ich.StreamLoadMore(pagingInfo));
            }
        },

        onNewStreamItemReceived: function (streamItem) {
            if (_.any(this.newStreamItemsCache, function (item) { return item.id === streamItem.id; }, this)) {
                return;
            }
            this.$el.find('.stream-message').remove();
            var streamItemCreatedDateTime = moment(streamItem.get('CreatedDateTime'), 'YYYY-MM-DDTHH:mm:ssZ');

            // Only show a new items message if the item is newer than what we have already
            if (streamItemCreatedDateTime.isAfter(moment(this.collection.baselineDateTime))) {
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
            this.$el.find('.stream-message, .stream-load-new, .stream-load-more, .no-more-items').fadeOut(100);
            this.$el.find('.activity-items').fadeOut(100, function () {
                that.onLoadingStart();
            });
        },

        beforeClose: function () {
            this.$el.find('.activity-list').infinitescroll('destroy');
        }
    });

    return ActivityListView;

});