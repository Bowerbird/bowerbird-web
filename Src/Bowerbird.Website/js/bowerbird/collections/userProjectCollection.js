/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// UserProjectCollection
// ---------------------

define(['jquery', 'underscore', 'backbone', 'collections/paginatedcollection', 'models/userproject'],
function ($, _, Backbone, PaginatedCollection, UserProject) {

    var UserProjectCollection = PaginatedCollection.extend({

        model: UserProject,

        baseUrl: '/userprojects',

        initialize: function (items, options) {
            _.bindAll(this, 'getFetchOptions');

            PaginatedCollection.prototype.initialize.apply(this, arguments);

            typeof (options) != 'undefined' || (options = {});

            this.page = options && options.page ? options.page : 1;
            this.pageSize = options && options.pageSize ? options.pageSize : 15;
            this.total = options && options.total ? options.total : 0;
            this.sortByType = options && options.sortBy ? options.sortBy : 'a-z';

            this.query = options && options.query ? options.query : '';
            this.field = options && options.field ? options.field : '';
        },

        comparator: function(userProject) {
            if (this.sortByType === 'z-a') {
                return String.fromCharCode.apply(String,
                    _.map(userProject.get('Name').toLowerCase().split(''), function (c) {
                        return 0xffff - c.charCodeAt();
                    })
                );
            } else {
                return userProject.get('Name');
            }
        },

        parse: function (resp) {
            var userProjects = resp.Model.UserProjects;
            this.page = userProjects.Page;
            this.pageSize = userProjects.PageSize;
            this.total = userProjects.TotalResultCount;
            return userProjects.PagedListItems;
        },

        fetchFirstPage: function () {
            this.firstPage(this.getFetchOptions(false));
        },

        fetchNextPage: function () {
            this.nextPage(this.getFetchOptions(true));
        },

        getFetchOptions: function (add) {
            var options = {
                data: {
                    sort: this.sortByType,
                    query: this.query,
                    field: this.field
                },
                add: add,
                success: null
            };
            if (add) {
                options.success = this.onSuccess;
            } else {
                options.success = this.onSuccessWithAddFix;
            }
            return options;
        },

        changeSort: function (sortByType) {
            if (this.sortByType !== sortByType) {
                this.trigger('criteria-changed');
                this.sortByType = sortByType;
                this.fetchFirstPage();
            }
        },

        changeQuery: function (query, field) {
            if (this.query !== query || this.field !== field) {
                this.trigger('criteria-changed');
                this.query = query;
                this.field = field;
                this.fetchFirstPage();
            }
        },

        hasSearchCriteria: function () {
            return this.query !== '';
        },

        searchUrl: function () {
            var url = this.baseUrl;

            var urlBits = [];

            if (this.sortByType !== 'a-z') {
                urlBits.push('sort=' + this.sortByType);
            }

            if (this.query !== '') {
                urlBits.push('query=' + this.query);
                if (this.field !== '') {
                    urlBits.push('field=' + this.field);
                }
            }

            if (urlBits.length > 0) {
                url = url + '?' + urlBits.join('&');
            }

            return url;
        },

        resetSearch: function () {
            this.trigger('search-reset');
            this.query = '';
            this.field = '',
            this.fetchFirstPage();
        }
    });

    return UserProjectCollection;

});