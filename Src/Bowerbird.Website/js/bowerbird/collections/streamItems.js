
window.Bowerbird.Collections.StreamItems = Bowerbird.Collections.PaginatedCollection.extend({
    model: Bowerbird.Models.StreamItem,

    baseUrl: '/members/streamitem/list',

    initialize: function () {
        _.extend(this, Backbone.Events);
        _.bindAll(this, '_onSuccess', '_onSuccessWithAddFix', '_getFetchOptions');
        Bowerbird.Collections.PaginatedCollection.prototype.initialize.apply(this, arguments);
    },

    fetchFirstPage: function (stream) {
        this._firstPage(this._getFetchOptions(stream, false));
    },

    fetchNextPage: function (stream) {
        this._nextPage(this._getFetchOptions(stream, true));
    },

    _getFetchOptions: function (stream, add) {
        var options = {
            data: {},
            add: add,
            success: null
        };
        if (add) {
            options.success = this._onSuccess;
        } else {
            options.success = this._onSuccessWithAddFix;
        }
        if (stream.get('context') != null) {
            if (stream.get('context') instanceof Bowerbird.Models.Team || stream.get('context') instanceof Bowerbird.Models.Project) {
                options.data.groupId = stream.get('context').get('id');
            } else if (stream.get('context') instanceof Bowerbird.Models.User) {
                options.data.userId = stream.get('context').get('id');
            }
        }
        if (stream.get('filter') != null) {
            options.data.filter = stream.get('filter');
        }
        return options;
    },

    _onSuccess: function (collection, response) {
        app.stream.trigger('fetchingItemsComplete', app.stream, response);
    },

    _onSuccessWithAddFix: function (collection, response) {
        this._onSuccess(collection, response);
        // Added the following manual triggering of 'add' event due to Backbone bug: https://github.com/documentcloud/backbone/issues/479
        var self = this;
        response.forEach(function (item) {
            self.trigger('add', item);
        });
    }
});