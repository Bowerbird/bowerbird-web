/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// OrganisationSearchPanelView
// ---------------------------

define(['jquery', 'underscore', 'backbone', 'ich', 'app', 'tipsy'],
function ($, _, Backbone, ich, app) {

    var OrganisationSearchPanelView = Backbone.Marionette.ItemView.extend({
        template: 'OrganisationSearchPanel',

        events: {
            'change #Category': 'categoryChanged',
            'click #search-go-button': 'queryChanged',
            'click #new-search-button': 'newSearch'
        },

        initialize: function (options) {
            this.organisationCollection = options.organisationCollection;
            this.categorySelectList = options.categorySelectList;
            this.fieldSelectList = options.fieldSelectList;
        },

        serializeData: function () {
            return {
                Model: {
                    CategorySelectList: this.categorySelectList,
                    FieldSelectList: this.fieldSelectList,
                    Query: {
                        Id: this.organisationCollection.projectId,
                        Page: this.organisationCollection.pageSize,
                        PageSize: this.organisationCollection.page,
                        View: this.organisationCollection.viewType,
                        Sort: this.organisationCollection.sortByType
                    }
                }
            };
        },

        onShow: function () {
            this._showDetails();
        },

        showBootstrappedDetails: function () {
            this._showDetails();
        },

        _showDetails: function () {
        },

        categoryChanged: function (e) {
            this.organisationCollection.changeCategory($(e.currentTarget).val());

            Backbone.history.navigate(this.organisationCollection.searchUrl(), { trigger: false });
        },

        queryChanged: function (e) {
            e.preventDefault();
            this.organisationCollection.changeQuery(this.$el.find('#Query').val(), this.$el.find('#Field').val());

            Backbone.history.navigate(this.organisationCollection.searchUrl(), { trigger: false });
            return false;
        },

        newSearch: function (e) {
            e.preventDefault();

            this.organisationCollection.resetSearch();

            Backbone.history.navigate(this.organisationCollection.searchUrl(), { trigger: false });

            this.$el.find('#Category').val('');
            this.$el.find('#Query').val('');
            this.$el.find('#Field').val('');

            return false;
        }

    });

    return OrganisationSearchPanelView;

});