
window.Bowerbird.Collections.StreamItems = Bowerbird.Collections.PaginatedCollection.extend({
    
    model: Bowerbird.Models.StreamItem,

    baseUrl: '/streamitem/list',

    initialize: function () {
        _.extend(this, Backbone.Events);
        _.bindAll(this,
        '_onSuccess',
        '_onSuccessWithAddFix',
        '_getFetchOptions'
        );
        Bowerbird.Collections.PaginatedCollection.prototype.initialize.apply(this, arguments);
    },

    comparator: function (streamItem1, streamItem2) {
        var streamItem1CreateDate = new Date(parseInt(streamItem1.get('CreatedDateTime').substr(6)));
        var streamItem2CreateDate = new Date(parseInt(streamItem2.get('CreatedDateTime').substr(6)));

        if (streamItem1CreateDate.isAfter(streamItem2CreateDate)) {
            return -1;
        }

        if (streamItem1CreateDate.isBefore(streamItem2CreateDate)) {
            return 1;
        }

        return -1;
    },

    fetchFirstPage: function (stream) {
        this._firstPage(this._getFetchOptions(stream, false));
    },

    fetchNextPage: function (stream) {
        this._nextPage(this._getFetchOptions(stream, true));
    },

    _getFetchOptions: function (stream, add) {
        var options = {
            Data: {},
            Add: add,
            Success: null
        };
        if (add) {
            options.Success = this._onSuccess;
        } else {
            options.Success = this._onSuccessWithAddFix;
        }
        if (stream.get('Context') != null) {
            if (stream.get('Context') instanceof Bowerbird.Models.Team || stream.get('Context') instanceof Bowerbird.Models.Project) {
                options.Data.GroupId = stream.get('Context').get('Id');
            } else if (stream.get('Context') instanceof Bowerbird.Models.User) {
                options.Data.UserId = stream.get('Context').get('Id');
            }
        }
        if (stream.get('Filter') != null) {
            options.Data.Filter = stream.get('Filter');
        }
        return options;
    },

    _onSuccess: function (collection, response) {
        app.stream.trigger('fetchingItemsComplete', app.Stream, response);
    },

    _onSuccessWithAddFix: function (collection, response) {
        this._onSuccess(collection, response);
        // Added the following manual triggering of 'add' event due to Backbone bug: https://github.com/documentcloud/backbone/issues/479
        var self = this;
        response.each(function (item, index) {
            self.trigger('add', item, self, { Index: index });
        });
    }
});