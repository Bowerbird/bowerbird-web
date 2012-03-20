
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
        this.streamListItemViews = [];
        app.stream.streamItems.on('add', this.addStreamItem, this);
        app.stream.on('newStream', this.showNewStream, this);
        //app.stream.on('newStreamFilter', this.showNewStreamFilter, this);
        //app.stream.on('newStreamPage', this.showNewStreamPage, this);
        app.stream.on('fetchingItemsStarted', this.showStreamLoading, this);
        app.stream.on('fetchingItemsComplete', this.hideStreamLoading, this);
    },

    render: function () {
        $.tmpl('streamListTemplate').appendTo(this.$el);
        return this;
    },

    addStreamItem: function (streamItem) {
        var streamListItemView = new Bowerbird.Views.StreamListItemView({ streamItem: streamItem });
        this.streamListItemViews.push(streamListItemView);
        $('#stream-items').append(streamListItemView.render().el);
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

    showStreamLoading: function (stream) {
        $('#stream-load-more').remove();
        $('#stream-items').append($.tmpl('streamListLoadingTemplate', { text: 'Loading', showLoader: true }));
    },

    hideStreamLoading: function (stream, collection) {
        $('#stream-status').remove();
        if (collection.length == 0) {
            $('#stream-items').append($.tmpl('streamListLoadingTemplate', { text: 'No activity yet! Start now by adding an observation.', showLoader: false }));
        }
        if (collection.pageInfo().next) {
            $('#stream-list > div').append($.tmpl('streamLoadMoreTemplate'));
        }
    },

    loadNextStreamItems: function () {
        app.stream.setNextPage();
    }
});