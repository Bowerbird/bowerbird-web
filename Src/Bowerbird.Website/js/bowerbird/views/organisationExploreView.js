/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// OrganisationExploreView
// ------------------

define(['jquery', 'underscore', 'backbone', 'app', 'views/organisationlistview', 'views/organisationsearchpanelview'],
function ($, _, Backbone, app, OrganisationListView, OrganisationSearchPanelView) {

    var OrganisationExploreView = Backbone.Marionette.Layout.extend({
        viewType: 'details',

        className: 'organisations double',

        template: 'OrganisationExplore',

        regions: {
            search: '.search',
            list: '.list'
        },

        initialize: function (options) {
            _.bindAll(this, 'toggleSearchPanel');

            this.organisationCollection = options.organisationCollection;
            this.categorySelectList = options.categorySelectList;
            this.fieldSelectList = options.fieldSelectList;
        },

        serializeData: function () {
            return {
                Model: {
                    ShowOrganisationExploreWelcome: app.authenticatedUser ? _.contains(app.authenticatedUser.callsToAction, 'organisation-explore-welcome') : false
                }
            };
        },

        onShow: function () {
            var options = {
                collection: this.organisationCollection
            };

            var searchOptions = {
                organisationCollection: this.organisationCollection,
                categorySelectList: this.categorySelectList,
                fieldSelectList: this.fieldSelectList
            };

            if (app.isPrerenderingView('organisations')) {
                options['el'] = '.list > div';
                searchOptions['el'] = '.search > div';
            }

            var organisationListView = new OrganisationListView(options);
            var organisationSearchPanelView = new OrganisationSearchPanelView(searchOptions);

            if (app.isPrerenderingView('organisations')) {
                this.list.attachView(organisationListView);
                organisationListView.showBootstrappedDetails();

                this.search.attachView(organisationSearchPanelView);
                organisationSearchPanelView.showBootstrappedDetails();
            } else {
                this.list.show(organisationListView);

                this.search.show(organisationSearchPanelView);
            }

            this._showDetails();
        },

        showBootstrappedDetails: function () {
            this.initializeRegions();

            var options = {
                collection: this.organisationCollection
            };

            var searchOptions = {
                organisationCollection: this.organisationCollection,
                categorySelectList: this.categorySelectList,
                fieldSelectList: this.fieldSelectList
            };

            if (app.isPrerenderingView('organisations')) {
                options['el'] = '.list > div';
                searchOptions['el'] = '.search > div';
            }

            var organisationListView = new OrganisationListView(options);
            var organisationSearchPanelView = new OrganisationSearchPanelView(searchOptions);

            if (app.isPrerenderingView('organisations')) {
                this.list.attachView(organisationListView);
                organisationListView.showBootstrappedDetails();

                this.search.attachView(organisationSearchPanelView);
                organisationSearchPanelView.showBootstrappedDetails();
            } else {
                this.list.show(organisationListView);

                this.search.show(organisationSearchPanelView);
            }

            this._showDetails();
        },

        _showDetails: function () {
            var that = this;

            this.list.currentView.on('toggle-search', this.toggleSearchPanel);

            if (this.organisationCollection.hasSearchCriteria()) {
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

    return OrganisationExploreView;

}); 