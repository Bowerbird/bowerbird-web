/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// SightingCollection
// ------------------

define(['jquery', 'underscore', 'backbone', 'collections/paginatedcollection', 'models/sighting'],
function ($, _, Backbone, PaginatedCollection, Sighting) {

    var SightingCollection = PaginatedCollection.extend({

        model: Sighting,

        baseUrl: '/sightings',

        initialize: function (items, options) {
            _.bindAll(this, 'getFetchOptions');

            PaginatedCollection.prototype.initialize.apply(this, arguments);

            //typeof (options) != 'undefined' || (options = {});

            if (options.projectId) {
                this.baseUrl = '/' + options.projectId + '/sightings';

                this.projectId = '/' + options.projectId;
            }

            this.page = options && options.page ? options.page : 1;
            this.pageSize = options && options.pageSize ? options.pageSize : 15;
            this.total = options && options.total ? options.total : 0;
            this.sortBy = options && options.sortBy ? options.sortBy : 'latestadded';
            this.viewType = options && options.viewType ? options.viewType : 'thumbnails';
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
                    sort: this.sortBy
                },
                add: add,
                success: null
            };
            if (add) {
                options.success = this.onSuccess;
            } else {
                options.success = this.onSuccessWithAddFix;
            }
            //            if (this.groupOrUser) {
            //                //if (this.groupOrUser instanceof Organisation || this.groupOrUser instanceof Team || this.groupOrUser instanceof Project) {
            //                if (this.groupOrUser instanceof Project) {
            //                    options.data.groupId = this.groupOrUser.id;
            //                } else if (this.groupOrUser instanceof User) {
            //                    options.data.userId = this.groupOrUser.id;
            //                }
            //            }
            //            //            if (stream.get('Filter') != null) {
            //            //                options.data.filter = stream.get('Filter');
            //            //            }
            return options;
        }
    });

    return SightingCollection;

});