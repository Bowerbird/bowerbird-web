
window.Bowerbird.Models.Stream = Backbone.Model.extend({
    defaults: {
        context: null,
        filter: null,
        uri: null
    },

    initialize: function (options) {
        _.extend(this, Backbone.Events);
        _.bindAll(this);
        this.streamItems = new Bowerbird.Collections.StreamItems();
    },

    isSet: function () {
        return this.get('filter') != null && this.get('uri') != null;
    },

    setNewStream: function (streamContext, streamFilter) {
        this.set('context', streamContext);
        this.set('filter', streamFilter);
        var uri = '';
        if (this.has('context')) {
            uri = this.get('context').get('id');
        }
        //uri += '/' + this.get('filter');
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
    },

    // Add stream items manually into stream (used by notification router)
    addStreamItem: function (streamItem) {
        var add = false;
        // Determine if user is currently viewing a relevant stream
        if (this.get('context') == null) {
            // Home stream
            add = true;
        } else {
            // Group stream
            var self = this;
            add = _.any(streamItem.groups, function (groupId) {
                return groupId === self.get('context').get('id');
            });
        }
        if (add) {
            this.streamItems.add(streamItem);
        }
    }
});