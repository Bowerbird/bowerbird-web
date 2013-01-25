/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// PostCollection
// --------------

define(['jquery', 'underscore', 'backbone', 'collections/paginatedcollection', 'models/post'],
function ($, _, Backbone, PaginatedCollection, Post) {

    var PostCollection = PaginatedCollection.extend({

        model: Post,

        baseUrl: '/posts',

        initialize: function (items, options) {
            _.bindAll(this, 'getFetchOptions');

            PaginatedCollection.prototype.initialize.apply(this, arguments);

            if (options.groupId) {
                this.baseUrl = '/' + options.groupId + '/posts';
                this.groupId = options.groupId;
            } else {
                this.baseUrl = '/home/posts';
            }

            this.page = options && options.page ? options.page : 1;
            this.pageSize = options && options.pageSize ? options.pageSize : 15;
            this.total = options && options.total ? options.total : 0;
            this.sortByType = options && options.sortBy ? options.sortBy : 'newest';
        },

        parse: function (resp) {
            var posts = resp.Model.Posts;
            this.page = posts.Page;
            this.pageSize = posts.PageSize;
            this.total = posts.TotalResultCount;
            return resp.Model.Posts.PagedListItems;
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

    return PostCollection;

});