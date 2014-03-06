/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ProjectExploreView
// ------------------

define(['jquery', 'underscore', 'backbone', 'app', 'views/projectlistview', 'views/projectsearchpanelview'],
function ($, _, Backbone, app, ProjectListView, ProjectSearchPanelView) {

    var ProjectExploreView = Backbone.Marionette.Layout.extend({
        viewType: 'details',

        className: 'projects double',

        template: 'ProjectExplore',

        regions: {
            search: '.search',
            list: '.list'
        },

        initialize: function (options) {
            _.bindAll(this, 'toggleSearchPanel');

            this.projectCollection = options.projectCollection;
            this.categorySelectList = options.categorySelectList;
            this.fieldSelectList = options.fieldSelectList;
        },

        serializeData: function () {
            return {
                Model: {
                    ShowProjectExploreWelcome: app.authenticatedUser ? _.contains(app.authenticatedUser.callsToAction, 'project-explore-welcome') : false
                }
            };
        },

        onShow: function () {
            var options = {
                collection: this.projectCollection
            };

            var searchOptions = {
                projectCollection: this.projectCollection,
                categorySelectList: this.categorySelectList,
                fieldSelectList: this.fieldSelectList
            };

            if (app.isPrerenderingView('projects')) {
                options['el'] = '.list > div';
                searchOptions['el'] = '.search > div';
            }

            var projectListView = new ProjectListView(options);
            var projectSearchPanelView = new ProjectSearchPanelView(searchOptions);

            if (app.isPrerenderingView('projects')) {
                this.list.attachView(projectListView);
                projectListView.showBootstrappedDetails();

                this.search.attachView(projectSearchPanelView);
                projectSearchPanelView.showBootstrappedDetails();
            } else {
                this.list.show(projectListView);

                this.search.show(projectSearchPanelView);
            }

            this._showDetails();
        },

        showBootstrappedDetails: function () {
            this.initializeRegions();

            var options = {
                collection: this.projectCollection
            };

            var searchOptions = {
                projectCollection: this.projectCollection,
                categorySelectList: this.categorySelectList,
                fieldSelectList: this.fieldSelectList
            };

            if (app.isPrerenderingView('projects')) {
                options['el'] = '.list > div';
                searchOptions['el'] = '.search > div';
            }

            var projectListView = new ProjectListView(options);
            var projectSearchPanelView = new ProjectSearchPanelView(searchOptions);

            if (app.isPrerenderingView('projects')) {
                this.list.attachView(projectListView);
                projectListView.showBootstrappedDetails();

                this.search.attachView(projectSearchPanelView);
                projectSearchPanelView.showBootstrappedDetails();
            } else {
                this.list.show(projectListView);

                this.search.show(projectSearchPanelView);
            }

            this._showDetails();
        },

        _showDetails: function () {
            var that = this;

            this.list.currentView.on('toggle-search', this.toggleSearchPanel);

            if (this.projectCollection.hasSearchCriteria()) {
                this.$el.find('.search-bar').slideDown();
            }

            this.$el.find('.close-call-to-action').on('click', function (e) {
                e.preventDefault();
                that.$el.find('.call-to-action').slideUp('fast', function () {
                    that.$el.find('.call-to-action').remove();
                });
                app.vent.trigger('close-call-to-action', 'user-welcome');
                return false;
            });
        },

        toggleSearchPanel: function () {
            log('search bar', this.$el.find('.search-bar'));
            if (this.$el.find('.search-bar').is(':visible')) {
                log('search is visible');
                this.$el.find('.search-bar').slideToggle();
            } else {
                log('search is not visible');
                var that = this;
                this.$el.find('.search-bar').slideToggle(function () {
                    that.$el.find('.search-bar #query').focus();
                });
            }
        }
    });

    return ProjectExploreView;

}); 