/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// SightingCollection
// ------------------

define(['jquery', 'underscore', 'backbone', 'collections/paginatedcollection', 'models/sighting', 'moment'],
function ($, _, Backbone, PaginatedCollection, Sighting, moment) {

    var SightingCollection = PaginatedCollection.extend({

        model: Sighting,

        baseUrl: '/sightings',

        initialize: function (items, options) {
            _.bindAll(this, 'getFetchOptions');

            PaginatedCollection.prototype.initialize.apply(this, arguments);

            if (options.subId) {
                this.baseUrl = '/' + options.subId + '/sightings';
                this.subId = options.subId;
            }

            this.page = options && options.page ? options.page : 1;
            this.pageSize = options && options.pageSize ? options.pageSize : 15;
            this.total = options && options.total ? options.total : 0;
            this.sortByType = options && options.sortBy ? options.sortBy : 'newest';
            this.viewType = options && options.viewType ? options.viewType : 'thumbnails';

            this.category = options && options.category ? options.category : '';
            this.needsId = options && options.needsId ? options.needsId : false;
            this.query = options && options.query ? options.query : '';
            this.field = options && options.field ? options.field : '';
            this.taxonomy = options && options.taxonomy ? options.taxonomy : '';
        },

        comparator: function (sighting) {
            if (this.sortByType === 'z-a') {
                return String.fromCharCode.apply(String,
                    _.map(sighting.get('Title').toLowerCase().split(''), function (c) {
                        return 0xffff - c.charCodeAt();
                    })
                );
            } else if (this.sortByType === 'a-z') {
                return sighting.get('Title');
            } else if (this.sortByType === 'oldest') {
                return parseInt(moment(sighting.get('CreatedOn')).format('YYYYMMDDTHHmmss'));
            } else if (this.sortByType === 'newest') {
                return -parseInt(moment(sighting.get('CreatedOn')).format('YYYYMMDDTHHmmss'));
            } else {
                return -parseInt(moment(sighting.get('CreatedOn')).format('YYYYMMDDTHHmmss'));
            }
        },

        parse: function (resp) {
            var sightings = resp.Model.Sightings;
            this.page = sightings.Page;
            this.pageSize = sightings.PageSize;
            this.total = sightings.TotalResultCount;
            return resp.Model.Sightings.PagedListItems;
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
                    view: this.viewType,
                    sort: this.sortByType,
                    category: this.category,
                    needsId: this.needsId,
                    query: this.query,
                    field: this.field,
                    taxonomy: this.taxonomy
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

        changeView: function (viewType) {
            if (this.viewType !== viewType) {
                this.trigger('criteria-changed');
                this.viewType = viewType;
                this.fetchFirstPage();
            }
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

        changeNeedsId: function (needsId) {
            if (this.needsId !== needsId) {
                this.trigger('criteria-changed');
                this.needsId = needsId;
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

        changeTaxonomy: function (taxonomy) {
            if (this.taxonomy !== taxonomy) {
                this.trigger('criteria-changed');
                this.taxonomy = taxonomy;
                this.fetchFirstPage();
            }
        },

        hasSearchCriteria: function () {
            return this.category !== '' ||
                this.needsId === true ||
                this.taxonomy !== '' ||
                this.query !== '';
        },

        searchUrl: function () {
            var url = this.baseUrl;

            var urlBits = [];

            if (this.sortByType !== 'newest') {
                urlBits.push('sort=' + this.sortByType);
            }

            if (this.viewType !== 'thumbnails') {
                urlBits.push('view=' + this.viewType);
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
            if (this.needsId !== false) {
                urlBits.push('needsid=' + this.needsId);
            }
            if (this.taxonomy !== '') {
                urlBits.push('taxonomy=' + this.taxonomy);
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
            this.taxonomy = '';
            this.needsId = false;
            this.fetchFirstPage();
        }
    });

    return SightingCollection;

});