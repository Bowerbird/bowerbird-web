/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ProjectSearchPanelView
// ----------------------

define(['jquery', 'underscore', 'backbone', 'ich', 'app', 'tipsy'],
function ($, _, Backbone, ich, app) {

    var ProjectSearchPanelView = Backbone.Marionette.ItemView.extend({
        template: 'ProjectSearchPanel',

        events: {
            'change #Category': 'categoryChanged',
            'click #search-go-button': 'queryChanged',
            'click #new-search-button': 'newSearch'
        },

        initialize: function (options) {
            this.projectCollection = options.projectCollection;
            this.categorySelectList = options.categorySelectList;
            this.fieldSelectList = options.fieldSelectList;
        },

        serializeData: function () {
            return {
                Model: {
                    CategorySelectList: this.categorySelectList,
                    FieldSelectList: this.fieldSelectList,
                    Query: {
                        Id: this.projectCollection.projectId,
                        Page: this.projectCollection.pageSize,
                        PageSize: this.projectCollection.page,
                        View: this.projectCollection.viewType,
                        Sort: this.projectCollection.sortByType
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
            this.projectCollection.changeCategory($(e.currentTarget).val());

            Backbone.history.navigate(this.projectCollection.searchUrl(), { trigger: false });
        },

        queryChanged: function (e) {
            e.preventDefault();
            this.projectCollection.changeQuery(this.$el.find('#Query').val(), this.$el.find('#Field').val());

            Backbone.history.navigate(this.projectCollection.searchUrl(), { trigger: false });
            return false;
        },

        newSearch: function (e) {
            e.preventDefault();

            this.projectCollection.resetSearch();

            Backbone.history.navigate(this.projectCollection.searchUrl(), { trigger: false });

            this.$el.find('#Category').val('');
            this.$el.find('#Query').val('');
            this.$el.find('#Field').val('');

            return false;
        }

    });

    return ProjectSearchPanelView;

});