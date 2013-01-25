/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// OrganisationCollection
// ----------------------

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
                return parseInt(organisation.get('UserCount'));
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

    return OrganisationCollection;

});