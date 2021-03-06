﻿/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ActivityCollection
// ------------------

define(['jquery', 'underscore', 'backbone', 'collections/paginatedcollection', 'models/activity', 'models/user', 'models/project', 'models/organisation'], function ($, _, Backbone, PaginatedCollection, Activity, User, Project, Organisation) {

    var padDateTime = function (val) {
        if (val < 10) {
            return '0' + val.toString();
        }
        return val.toString();
    };

    var ActivityCollection = PaginatedCollection.extend({
        model: Activity,

        baseUrl: '/',

        groupOrUser: null,

        initialize: function (models, options) {
            _.bindAll(this, 'onSuccess', 'onSuccessWithAddFix', 'getFetchOptions');

            PaginatedCollection.prototype.initialize.apply(this, arguments);

            typeof (options) != 'undefined' || (options = {});

            if (options.id) {
                this.baseUrl = '/' + options.id;
            }

            // Set the moment in time (UTC) around which all activity queries will be performed
            var now = new Date();
            this.baselineDateTime = new Date(now.getUTCFullYear(), now.getUTCMonth(), now.getUTCDate(), now.getUTCHours(), now.getUTCMinutes(), now.getUTCSeconds());
        },

        parse: function (resp) {
            var activities = null;
            if (this.groupOrUser) {
                activities = resp.Model;
            } else {
                activities = resp.Model.Activities;
            }
            this.page = activities.Page;
            this.pageSize = activities.PageSize;
            this.total = activities.TotalResultCount;
            return activities.PagedListItems;
        },

        comparator: function (activity) {
            return -parseInt(activity.get('CreatedDateTimeOrder'));
        },

        fetchFirstPage: function () {
            this.firstPage(this.getFetchOptions(true, 'olderThan'));
        },

        fetchNextPage: function () {
            this.nextPage(this.getFetchOptions(true, 'olderThan'));
        },

        fetchNewItems: function () {
            this.fetch(this.getFetchOptions(true, 'newerThan'));
        },

        getFetchOptions: function (add, newerOrOlder) {
            var options = {
                data: {},
                add: add,
                success: null
            };
            if (add) {
                options.success = this.onSuccess;
            } else {
                options.success = this.onSuccessWithAddFix;
            }
            if (this.groupOrUser) {
                if (this.groupOrUser instanceof Project || this.groupOrUser instanceof Organisation) {
                    options.data.groupId = this.groupOrUser.id;
                } else if (this.groupOrUser instanceof User) {
                    options.data.userId = this.groupOrUser.id;
                }
            }

            // Get either items newer than or older than baseline
            options.data[newerOrOlder] = this.baselineDateTime.getFullYear() + '-' + padDateTime(this.baselineDateTime.getMonth() + 1) + '-' + padDateTime(this.baselineDateTime.getDate()) + 'T' + padDateTime(this.baselineDateTime.getHours()) + ':' + padDateTime(this.baselineDateTime.getMinutes()) + ':' + padDateTime(this.baselineDateTime.getSeconds()) + 'Z';

            return options;
        },

        onSuccess: function (collection, response) {
            //app.stream.trigger('fetchingItemsComplete', app.stream, response);
        },

        onSuccessWithAddFix: function (collection, response) {
            this.onSuccess(collection, response);
            // Added the following manual triggering of 'add' event due to Backbone bug: https://github.com/documentcloud/backbone/issues/479
            var self = this;
            response.each(function (item, index) {
                self.trigger('add', item, self, { Index: index });
            });
        },

        searchUrl: function (includePagination, pageNumber) {
            // Eg: http://localhost:65061/?page=2&pageSize=10&olderThan=2013-04-24T04%3A43%3A08Z
            
            var url = this.baseUrl;

            var urlBits = [];

            urlBits.push('olderthan=' + this.baselineDateTime.getFullYear() + '-' + padDateTime(this.baselineDateTime.getMonth() + 1) + '-' + padDateTime(this.baselineDateTime.getDate()) + 'T' + padDateTime(this.baselineDateTime.getHours()) + ':' + padDateTime(this.baselineDateTime.getMinutes()) + ':' + padDateTime(this.baselineDateTime.getSeconds()) + 'Z');

            if (includePagination) {
                urlBits.push('pagesize=10');
                urlBits.push('page=' + pageNumber);
            }

            if (urlBits.length > 0) {
                url = url + '?' + urlBits.join('&');
            }

            return url;
        }
    });

    return ActivityCollection;

});