/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// StreamView
// ----------

// Shows stream items for selected user/group
define(['jquery', 'underscore', 'backbone', 'app', 'ich', 'views/streamitemview'],
function ($, _, Backbone, app, ich, StreamItemView) 
{
    var StreamView = Backbone.Marionette.CompositeView.extend({
        template: 'Stream',

        itemView: StreamItemView,

        className: 'stream',

        events: {
            "click #stream-load-more-button": "loadNextStreamItems"
        },

        initialize: function (options) {
            this.collection.on('fetching', this.onStreamLoadingStart, this);
            this.collection.on('fetched', this.onsStreamLoadingComplete, this);
        },

        showBootstrappedDetails: function () {
        },

        appendHtml: function (collectionView, itemView) {
            collectionView.$el.find('.stream-list').append(itemView.el);
        },

        onStreamLoadingStart: function (collection) {
            this.$el.find('#stream-load-more').remove();
            this.$el.find('.stream-list').append(ich.StreamListLoading({ Text: 'Loading', ShowLoader: true }));
        },

        onsStreamLoadingComplete: function (collection) {
            this.toggleNoStreamItemsStatus(collection);
            if (collection.pageInfo().next) {
                this.$el.find('.stream-list > div').append(ich.StreamListLoading());
            }
        },

        toggleNoStreamItemsStatus: function (collection) {
            this.$el.find('#stream-status').remove();
            if (collection.length === 0) {
                this.$el.find('.stream-list > div').append(ich.StreamListLoading({ Text: 'No activity yet! Start now by adding an observation.' }));
            }
        },

        loadNextStreamItems: function () {
            app.stream.setNextPage();
        }
    });

    return StreamView;

});