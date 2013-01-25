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

            if (options.projectId) {
                this.baseUrl = '/' + options.projectId + '/sightings';
                this.projectId = options.projectId;
            } else {
                this.baseUrl = '/home/sightings';
            }

            this.page = options && options.page ? options.page : 1;
            this.pageSize = options && options.pageSize ? options.pageSize : 15;
            this.total = options && options.total ? options.total : 0;
            this.sortByType = options && options.sortBy ? options.sortBy : 'newest';
            this.viewType = options && options.viewType ? options.viewType : 'thumbnails';
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
                    sort: this.sortByType
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
        }
    });

    return SightingCollection;

});