/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// PostSearchPanelView
// -------------------

define(['jquery', 'underscore', 'backbone', 'ich', 'app', 'tipsy'],
function ($, _, Backbone, ich, app) {

    var PostSearchPanelView = Backbone.Marionette.ItemView.extend({
        template: 'PostSearchPanel',

        events: {
            'click #search-go-button': 'queryChanged',
            'click #new-search-button': 'newSearch'
        },

        initialize: function (options) {
            this.postCollection = options.postCollection;
            this.fieldSelectList = options.fieldSelectList;
        },

        serializeData: function () {
            return {
                Model: {
                    FieldSelectList: this.fieldSelectList,
                    Query: {
                        Id: this.postCollection.groupId ? this.postCollection.groupId : null,
                        Page: this.postCollection.pageSize,
                        PageSize: this.postCollection.page,
                        View: this.postCollection.viewType,
                        Sort: this.postCollection.sortByType
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

        queryChanged: function (e) {
            e.preventDefault();
            this.postCollection.changeQuery(this.$el.find('#Query').val(), this.$el.find('#Field').val());

            Backbone.history.navigate(this.postCollection.searchUrl(), { trigger: false });
            return false;
        },

        newSearch: function (e) {
            e.preventDefault();

            this.postCollection.resetSearch();

            Backbone.history.navigate(this.postCollection.searchUrl(), { trigger: false });

            this.$el.find('#Query').val('');
            this.$el.find('#Field').val('');

            return false;
        }

    });

    return PostSearchPanelView;

});