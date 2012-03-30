
window.Bowerbird.Collections.PaginatedCollection = Backbone.Collection.extend({
    initialize: function () {
        _.extend(this, Backbone.Events);
        typeof (options) != 'undefined' || (options = {});
        this.page = 1;
        typeof (this.pageSize) != 'undefined' || (this.pageSize = 10);
    },

    fetch: function (options) {
        typeof (options) != 'undefined' || (options = {});
        this.trigger("fetching");
        var self = this;
        var success = options.success;
        options.success = function (resp) {
            self.trigger("fetched");
            if (success) { success(self, resp); }
        };
        return Backbone.Collection.prototype.fetch.call(this, options);
    },

    parse: function (resp) {
        this.page = resp.page;
        this.pageSize = resp.pageSize;
        this.total = resp.totalResultCount;
        return resp.pagedListItems;
    },

    url: function () {
        return this.baseUrl + '?' + $.param({ page: this.page, pageSize: this.pageSize });
    },

    pageInfo: function () {
        var info = {
            total: this.total,
            page: this.page,
            pageSize: this.pageSize,
            pages: Math.ceil(this.total / this.pageSize),
            prev: false,
            next: false
        };

        var max = Math.min(this.total, this.page * this.pageSize);

        if (this.total == this.pages * this.pageSize) {
            max = this.total;
        }

        info.range = [(this.page - 1) * this.pageSize + 1, max];

        if (this.page > 1) {
            info.prev = this.page - 1;
        }

        if (this.page < info.pages) {
            info.next = this.page + 1;
        }

        return info;
    },

    _firstPage: function (options) {
        this.page = 1;
        return this.fetch(options);
    },

    _nextPage: function (options) {
        if (!this.pageInfo().next) {
            return false;
        }
        this.page = this.page + 1;
        return this.fetch(options);
    },

    _previousPage: function () {
        if (!this.pageInfo().prev) {
            return false;
        }
        this.page = this.page - 1;
        return this.fetch(options);
    }
});
