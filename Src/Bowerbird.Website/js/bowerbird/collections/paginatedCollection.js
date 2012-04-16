
window.Bowerbird.Collections.PaginatedCollection = Backbone.Collection.extend({
    initialize: function () {
        _.extend(this, Backbone.Events);
        typeof (options) != 'undefined' || (options = {});
        this.Page = 1;
        typeof (this.PageSize) != 'undefined' || (this.PageSize = 10);
    },

    fetch: function (options) {
        typeof (options) != 'undefined' || (options = {});
        this.trigger("fetching");
        var self = this;
        var success = options.Success;
        options.Success = function (resp) {
            self.trigger("fetched");
            if (success) { success(self, resp); }
        };
        return Backbone.Collection.prototype.fetch.call(this, options);
    },

    parse: function (resp) {
        this.Page = resp.Page;
        this.PageSize = resp.PageSize;
        this.Total = resp.TotalResultCount;
        return resp.PagedListItems;
    },

    Url: function () {
        return this.baseUrl + '?' + $.param({ Page: this.Page, PageSize: this.PageSize });
    },

    pageInfo: function () {
        var info = {
            Total: this.Total,
            Page: this.Page,
            PageSize: this.PageSize,
            Pages: Math.ceil(this.Total / this.PageSize),
            Prev: false,
            Pext: false
        };

        var max = Math.min(this.Total, this.Page * this.PageSize);

        if (this.Total == this.Pages * this.PageSize) {
            max = this.Total;
        }

        info.range = [(this.Page - 1) * this.PageSize + 1, max];

        if (this.Page > 1) {
            info.Prev = this.Page - 1;
        }

        if (this.Page < info.Pages) {
            info.Next = this.Page + 1;
        }

        return info;
    },

    _firstPage: function (options) {
        this.Page = 1;
        return this.fetch(options);
    },

    _nextPage: function (options) {
        if (!this.pageInfo().Next) {
            return false;
        }
        this.Page = this.Page + 1;
        return this.fetch(options);
    },

    _previousPage: function () {
        if (!this.pageInfo().Prev) {
            return false;
        }
        this.Page = this.Page - 1;
        return this.fetch(options);
    }
});
