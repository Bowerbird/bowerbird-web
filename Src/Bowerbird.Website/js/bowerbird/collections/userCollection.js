/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// UserCollection
// --------------

define(['jquery', 'underscore', 'backbone', 'collections/paginatedcollection', 'models/user'],
function ($, _, Backbone, PaginatedCollection, User) {

    var UserCollection = PaginatedCollection.extend({

        model: User,

        baseUrl: '/users',

        initialize: function (items, options) {
            _.bindAll(this, 'getFetchOptions');

            PaginatedCollection.prototype.initialize.apply(this, arguments);

            typeof (options) != 'undefined' || (options = {});

            if (options.projectId) {
                this.isProjectUsers = true;
                this.baseUrl = '/' + options.projectId + '/members';
                this.projectId = '/' + options.projectId;
            } else {
                this.baseUrl = '/users';
            }

            this.page = options && options.page ? options.page : 1;
            this.pageSize = options && options.pageSize ? options.pageSize : 15;
            this.total = options && options.total ? options.total : 0;
            this.sortByType = options && options.sortBy ? options.sortBy : 'a-z';
        },

        comparator: function(user) {
            if (this.sortByType === 'z-a') {
                var str = user.get("Name");
                str = str.toLowerCase();
                str = str.split("");
                str = _.map(str, function (letter) {
                    return String.fromCharCode(-(letter.charCodeAt(0)));
                });
                return str;
            } else {
                return user.get('Name');
            }
        },

        parse: function (resp) {
            if (this.isProjectUsers) {
                var sightings = resp.Model.Sightings;
                this.page = sightings.Page;
                this.pageSize = sightings.PageSize;
                this.total = sightings.TotalResultCount;
                return resp.Model.Sightings.PagedListItems;
            }
            return [];
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

    return UserCollection;

});