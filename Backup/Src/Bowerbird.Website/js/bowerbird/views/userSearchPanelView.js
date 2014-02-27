/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// UserSearchPanelView
// -------------------

define(['jquery', 'underscore', 'backbone', 'ich', 'app', 'tipsy'],
function ($, _, Backbone, ich, app) {

    var UserSearchPanelView = Backbone.Marionette.ItemView.extend({
        template: 'UserSearchPanel',

        events: {
            'click #search-go-button': 'queryChanged',
            'click #new-search-button': 'newSearch'
        },

        initialize: function (options) {
            this.userCollection = options.userCollection;
            this.fieldSelectList = options.fieldSelectList;
        },

        serializeData: function () {
            return {
                Model: {
                    FieldSelectList: this.fieldSelectList,
                    Query: {
                        Id: this.userCollection.subId,
                        Page: this.userCollection.pageSize,
                        PageSize: this.userCollection.page,
                        View: this.userCollection.viewType,
                        Sort: this.userCollection.sortByType
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
            this.userCollection.changeQuery(this.$el.find('#Query').val(), this.$el.find('#Field').val());

            Backbone.history.navigate(this.userCollection.searchUrl(), { trigger: false });
            return false;
        },

        newSearch: function (e) {
            e.preventDefault();

            this.userCollection.resetSearch();

            Backbone.history.navigate(this.userCollection.searchUrl(), { trigger: false });

            this.$el.find('#Query').val('');
            this.$el.find('#Field').val('');

            return false;
        }

    });

    return UserSearchPanelView;

});