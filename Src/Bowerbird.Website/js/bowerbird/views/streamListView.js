
window.Bowerbird.Views.StreamListView = Backbone.View.extend({
    tagName: 'section',

    className: 'triple-2',

    id: 'stream-list',

    events: {
        "click #stream-load-more-button": "loadNextStreamItems"
    },

    initialize: function (options) {
        _.extend(this, Backbone.Events);
        _.bindAll(this, 'addStreamItem');
        this.streamListItemViews = [];
        app.stream.streamItems.on('add', this.addStreamItem, this);
        app.stream.on('newStream', this.showNewStream, this);
        //app.stream.on('newStreamFilter', this.showNewStreamFilter, this);
        //app.stream.on('newStreamPage', this.showNewStreamPage, this);
        app.stream.on('fetchingItemsStarted', this.onStreamLoadingStart, this);
        app.stream.on('fetchingItemsComplete', this.onsStreamLoadingComplete, this);
    },

    render: function () {
        var streamListHtml = ich.streamList().appendTo(this.$el);
        return this;
    },

    addStreamItem: function (streamItem, collection, options) {
        var streamListItemView = new Bowerbird.Views.StreamListItemView({ streamItem: streamItem });
        this.streamListItemViews.push(streamListItemView);
        this.toggleNoStreamItemsStatus(collection);
        if (options.index === 0) {
            $('#stream-items').prepend(streamListItemView.render().el);
        } else {
            $('#stream-items').append(streamListItemView.render().el);
        }
    },

    showNewStream: function (stream) {
        $('#stream-items').empty();
        $('#stream-load-more').remove();
        if (stream.get('context') instanceof Bowerbird.Models.Team) {
        } else if (stream.get('context') instanceof Bowerbird.Models.Project) {
        } else if (stream.get('context') instanceof Bowerbird.Models.User) {
        } else {

        }
    },

    onStreamLoadingStart: function (stream) {
        $('#stream-load-more').remove();
        var loadMoreHtml = ich.streamListLoading({ text: 'Loading', showLoader: true }).appendTo($('#stream-items'));
    },

    onsStreamLoadingComplete: function (stream, collection) {
        this.toggleNoStreamItemsStatus(collection);
        if (collection.pageInfo().next) {
            var loadMoreHtml = ich.streamListLoading().appendTo($('#stream-list > div'));
        }
    },

    toggleNoStreamItemsStatus: function (collection) {
        $('#stream-status').remove();
        if (collection.length === 0) {
            var loadMoreHtml = ich.streamListLoading({ text: 'No activity yet! Start now by adding an observation.' }).appendTo($('#stream-list > div'));
        }
    },

    loadNextStreamItems: function () {
        app.stream.setNextPage();
    }
});