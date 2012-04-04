
window.Bowerbird.Views.StreamListView = Backbone.View.extend({
    tagName: 'section',

    className: 'triple-2',

    id: 'stream-list',

    events: {
        "click #stream-load-more-button": "loadNextStreamItems"
    },

    template: $.template('streamListTemplate', $('#stream-list-template')),

    loadingTemplate: $.template('streamListLoadingTemplate', $('#stream-list-loading-template')),

    loadMoreTemplate: $.template('streamLoadMoreTemplate', $('#stream-load-more-template')),

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
        $.tmpl('streamListTemplate').appendTo(this.$el);
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
        $('#stream-items').append($.tmpl('streamListLoadingTemplate', { text: 'Loading', showLoader: true }));
    },

    onsStreamLoadingComplete: function (stream, collection) {
        this.toggleNoStreamItemsStatus(collection);
        if (collection.pageInfo().next) {
            $('#stream-list > div').append($.tmpl('streamLoadMoreTemplate'));
        }
    },

    toggleNoStreamItemsStatus: function (collection) {
        $('#stream-status').remove();
        if (collection.length === 0) {
            $('#stream-items').append($.tmpl('streamListLoadingTemplate', { text: 'No activity yet! Start now by adding an observation.', showLoader: false }));
        }
    },

    loadNextStreamItems: function () {
        app.stream.setNextPage();
    }
});