
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
        this.$el.append(ich.streamList());
        return this;
    },

    addStreamItem: function (streamItem, collection, options) {
        var streamListItemView = new Bowerbird.Views.StreamListItemView({ streamItem: streamItem });
        this.streamListItemViews.push(streamListItemView);
        this.toggleNoStreamItemsStatus(collection);
        if (options.Index === 0) {
            $('#stream-items').prepend(streamListItemView.render().el);
        } else {
            $('#stream-items').append(streamListItemView.render().el);
        }
    },

    showNewStream: function (stream) {
        $('#stream-items').empty();
        $('#stream-load-more').remove();
        if (stream.get('Context') instanceof Bowerbird.Models.Team) {
        } else if (stream.get('Context') instanceof Bowerbird.Models.Project) {
        } else if (stream.get('Context') instanceof Bowerbird.Models.User) {
        } else {

        }
    },

    onStreamLoadingStart: function (stream) {
        $('#stream-load-more').remove();
        $('#stream-items').append(ich.streamListLoading({ Text: 'Loading', ShowLoader: true }));
    },

    onsStreamLoadingComplete: function (stream, collection) {
        this.toggleNoStreamItemsStatus(collection);
        if (collection.pageInfo().next) {
            $('#stream-list > div').append(ich.streamListLoading());
        }
    },

    toggleNoStreamItemsStatus: function (collection) {
        $('#stream-status').remove();
        if (collection.length === 0) {
            $('#stream-list > div').append(ich.streamListLoading({ Text: 'No activity yet! Start now by adding an observation.' }));
        }
    },

    loadNextStreamItems: function () {
        app.stream.setNextPage();
    }
});