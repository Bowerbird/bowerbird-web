/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// OrganisationCollection
// -----------------

define(['jquery', 'underscore', 'backbone', 'collections/paginatedcollection', 'models/organisation'],
function ($, _, Backbone, PaginatedCollection, Organisation) {

    var OrganisationCollection = PaginatedCollection.extend({

        model: Organisation,

        baseUrl: '/organisations',

        initialize: function (items, options) {
            _.bindAll(this, 'getFetchOptions');

            PaginatedCollection.prototype.initialize.apply(this, arguments);

            typeof (options) != 'undefined' || (options = {});

            this.page = options && options.page ? options.page : 1;
            this.pageSize = options && options.pageSize ? options.pageSize : 15;
            this.total = options && options.total ? options.total : 0;
            this.sortByType = options && options.sortBy ? options.sortBy : 'popular';

            this.category = options && options.category ? options.category : '';
            this.query = options && options.query ? options.query : '';
            this.field = options && options.field ? options.field : '';
        },

        comparator: function (organisation) {
            if (this.sortByType === 'z-a') {
                return String.fromCharCode.apply(String,
                    _.map(organisation.get('Name').toLowerCase().split(''), function (c) {
                        return 0xffff - c.charCodeAt();
                    })
                );
            } else if (this.sortByType === 'a-z') {
                return organisation.get('Name');
            } else if (this.sortByType === 'oldest') {
                return parseInt(organisation.get('CreatedDateTimeOrder'));
            } else if (this.sortByType === 'newest') {
                return -parseInt(organisation.get('CreatedDateTimeOrder'));
            } else {
                return -parseInt(organisation.get('UserCount'));
            }
        },

        parse: function (resp) {
            var organisations = resp.Model.Organisations;
            this.page = organisations.Page;
            this.pageSize = organisations.PageSize;
            this.total = organisations.TotalResultCount;
            return organisations.PagedListItems;
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
                    category: this.category,
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

        changeCategory: function (category) {
            if (this.category !== category) {
                this.trigger('criteria-changed');
                this.category = category;
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
            return this.category !== '' ||
                this.query !== '';
        },

        searchUrl: function (includePagination, pageNumber) {
            var url = this.baseUrl;

            var urlBits = [];

            if (this.sortByType !== 'popular') {
                urlBits.push('sort=' + this.sortByType);
            }

            if (this.query !== '') {
                urlBits.push('query=' + this.query);
                if (this.field !== '') {
                    urlBits.push('field=' + this.field);
                }
            }

            if (this.category !== '') {
                urlBits.push('category=' + encodeURIComponent(this.category));
            }

            if (includePagination) {
                urlBits.push('pagesize=' + this.pageSize);
                urlBits.push('page=' + pageNumber);
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
            this.category = '';
            this.fetchFirstPage();
        }
    });

    return OrganisationCollection;

});