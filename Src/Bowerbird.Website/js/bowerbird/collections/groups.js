
window.Bowerbird.Collections.Groups = Bowerbird.Collections.PaginatedCollection.extend({
    
    model: Bowerbird.Models.Group,

    baseUrl: '',

    initialize: function (options) {
        _.extend(this, Backbone.Events);
        _.bindAll(this,
        '_onSuccess',
        '_onSuccessWithAddFix',
        '_getFetchOptions',
        '_setBaseUrl'
        );
        this.constructor.__super__.initialize.apply(this, options);
    },

    fetchFirstPage: function (explore) {
        this._setBaseUrl(explore);
        this._firstPage(this._getFetchOptions(explore, false));
    },

    fetchNextPage: function (explore) {
        this._setBaseUrl(explore);
        this._nextPage(this._getFetchOptions(explore, true));
    },

    _setBaseUrl: function (explore) {
        this.baseUrl = explore.get('Uri');
    },

    _getFetchOptions: function (explore, add) {
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
        //        if (explore.get('context') != null) {
        //            if (stream.get('context') instanceof Bowerbird.Models.Team || stream.get('context') instanceof Bowerbird.Models.Project) {
        //                options.data.groupId = stream.get('context').get('id');
        //            } else if (stream.get('context') instanceof Bowerbird.Models.User) {
        //                options.data.userId = stream.get('context').get('id');
        //            }
        //        }
        if (explore.get('Filter') != null) {
            options.Data.Filter = explore.get('Filter');
        }
        return options;
    },

    _onSuccess: function (collection, response) {
        app.explore.trigger('fetchingItemsComplete', app.Explore, response);
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