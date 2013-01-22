/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ProjectCollection
// -----------------

define(['jquery', 'underscore', 'backbone', 'collections/paginatedcollection', 'models/project'],
function ($, _, Backbone, PaginatedCollection, Project) {

    var ProjectCollection = PaginatedCollection.extend({

        model: Project,

        baseUrl: '/projects',

        initialize: function (items, options) {
            _.bindAll(this, 'getFetchOptions');

            PaginatedCollection.prototype.initialize.apply(this, arguments);

            typeof (options) != 'undefined' || (options = {});

            this.page = options && options.page ? options.page : 1;
            this.pageSize = options && options.pageSize ? options.pageSize : 15;
            this.total = options && options.total ? options.total : 0;
            this.sortByType = options && options.sortBy ? options.sortBy : 'newest';
        },

        comparator: function(project) {
            if (this.sortByType === 'z-a') {
                var str = project.get('Name');
                str = str.toLowerCase();
                str = str.split('');
                return _.map(str, function(letter) {
                    return String.fromCharCode(-(letter.charCodeAt(0)));
                });
            } else if (this.sortByType === 'a-z') {
                return project.get('Name');
            } else if (this.sortByType === 'oldest') {
                return project.get('CreatedDateTimeOrder');
            } else {
                return -parseInt(project.get('CreatedDateTimeOrder'));
            }
        },

        parse: function (resp) {
            var projects = resp.Model.Projects;
            this.page = projects.Page;
            this.pageSize = projects.PageSize;
            this.total = projects.TotalResultCount;
            return projects.PagedListItems;
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

    return ProjectCollection;

});