
window.Bowerbird.Models.Stream = Backbone.Model.extend({
    defaults: {
        context: null,
        filter: null,
        uri: ''
    },

    initialize: function (options) {
        _.extend(this, Backbone.Events);
        _.bindAll(this);
        this.streamItems = new Bowerbird.Collections.StreamItems();
    },

    setNewStream: function (context, filter) {
        this.set('context', context);
        this.set('filter', filter);
        var uri = '';
        if (this.has('context')) {
            uri = this.get('context').get('id');
        } else {
            uri = 'home';
        }
        uri += '/' + this.get('filter');
        this.set('uri', uri);
        this.trigger('newStream', this);
        this.trigger('fetchingItemsStarted', this);
        this.streamItems.fetchFirstPage(this);
    },

    setNewFilter: function (filter) {
        this.set('filter', filter);
        this.trigger('newStreamFilter', this);
        this.trigger('fetchingItemsStarted', this);
        this.streamItems.fetchFirstPage(this);
    },

    setNextPage: function () {
        this.trigger('newStreamPage', this);
        this.trigger('fetchingItemsStarted', this);
        this.streamItems.fetchNextPage(this);
    }
});