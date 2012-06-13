/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ExploreProjectView
// ------------------

// Shows Explore project items for selected Project
define([
'jquery',
'underscore',
'backbone',
'app',
'ich',
'views/exploreprojectitemview'],
function (
$,
_,
Backbone,
app,
ich, 
ExploreProjectItemView) {

    var ExploreProjectView = Backbone.Marionette.CompositeView.extend({
        
        template: 'ProjectList',

        itemView: ExploreProjectItemView,

        events: {
            "click #explore-load-more-button": "loadNextExploreItems"
        },

        initialize: function (options) {
            this.collection.on('fetching', this.onExploreLoadingStart, this);
            this.collection.on('fetched', this.onExploreLoadingComplete, this);
        },

        showBootstrappedDetails: function () {
        },

        appendHtml: function (collectionView, itemView) {
            collectionView.$el.find('.explore-list').append(itemView.el);
        },

        onExploreLoadingStart: function (collection) {
            this.$el.find('#explore-load-more').remove();
            this.$el.find('.explore-list').append(ich.StreamListLoading({ Text: 'Loading', ShowLoader: true }));
        },

        onExploreLoadingComplete: function (collection) {
            this.toggleNoExpolreItemsStatus(collection);
            if (collection.pageInfo().next) {
                this.$el.find('.explore-list > div').append(ich.StreamListLoading());
            }
        },

        toggleNoStreamItemsStatus: function (collection) {
            this.$el.find('#explore-status').remove();
            if (collection.length === 0) {
                this.$el.find('.explore-list > div').append(ich.StreamListLoading({ Text: 'No items yet!' }));
            }
        },

        loadNextExploreItems: function () {
            app.stream.setNextPage();
        }
    });

    return ExploreProjectView;

});