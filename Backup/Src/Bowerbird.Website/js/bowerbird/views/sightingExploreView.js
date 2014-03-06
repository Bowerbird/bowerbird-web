/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// SightingExploreView
// -------------------

define(['jquery', 'underscore', 'backbone', 'app', 'views/sightinglistview', 'views/sightingsearchpanelview'],
function ($, _, Backbone, app, SightingListView, SightingSearchPanelView) {

    var SightingExploreView = Backbone.Marionette.Layout.extend({
        viewType: 'details',

        className: 'sightings double',

        template: 'SightingExplore',

        regions: {
            search: '.search',
            list: '.list'
        },

        initialize: function (options) {
            _.bindAll(this, 'toggleSearchPanel');

            this.sightingCollection = options.sightingCollection;
            this.categorySelectList = options.categorySelectList;
            this.fieldSelectList = options.fieldSelectList;
        },

        serializeData: function () {
            return {
                Model: {
                }
            };
        },

        onShow: function () {
            var options = {
                collection: this.sightingCollection
            };

            var searchOptions = {
                sightingCollection: this.sightingCollection,
                categorySelectList: this.categorySelectList,
                fieldSelectList: this.fieldSelectList
            };

            //            if (app.isPrerenderingView('sightings')) {
            //                options['el'] = '.list > div';
            //                searchOptions['el'] = '.search > div';
            //            }

            this.sightingListView = new SightingListView(options);
            this.sightingSearchPanelView = new SightingSearchPanelView(searchOptions);

            //if (app.isPrerenderingView('sightings')) {
            //                            this.list.attachView(sightingListView);
            //                            sightingListView.showBootstrappedDetails();

            //                            this.search.attachView(sightingSearchPanelView);
            //                            sightingSearchPanelView.showBootstrappedDetails();
            //            } else {
            this.list.show(this.sightingListView);
            this.search.show(this.sightingSearchPanelView);
            //}


            this._showDetails();
        },

        showBootstrappedDetails: function () {
            this.initializeRegions();

            var options = {
                collection: this.sightingCollection
            };

            var searchOptions = {
                sightingCollection: this.sightingCollection,
                categorySelectList: this.categorySelectList,
                fieldSelectList: this.fieldSelectList
            };

            if (app.isPrerenderingView('sightings')) {
                options['el'] = '.list > div';
                searchOptions['el'] = '.search > div';
            }

            this.sightingListView = new SightingListView(options);
            this.sightingSearchPanelView = new SightingSearchPanelView(searchOptions);

            //            if (app.isPrerenderingView('sightings')) {
            this.list.attachView(this.sightingListView);
            this.sightingListView.showBootstrappedDetails();

            this.search.attachView(this.sightingSearchPanelView);
            this.sightingSearchPanelView.showBootstrappedDetails();
            //            } else {
            //                this.list.show(sightingListView);

            //                this.search.show(sightingSearchPanelView);
            //            }

            this._showDetails();
        },

        _showDetails: function () {
            var that = this;

            this.list.currentView.on('toggle-search', this.toggleSearchPanel);
            
            if (this.sightingCollection.hasSearchCriteria()) {
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

            app.vent.on('view:render:complete', function () {
                this.sightingListView.refresh();
            }, this);
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

    return SightingExploreView;

}); 