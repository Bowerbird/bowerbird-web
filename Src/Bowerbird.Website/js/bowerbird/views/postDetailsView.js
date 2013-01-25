/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ObservationDetailsView
// ----------------------

define(['jquery', 'underscore', 'backbone', 'app'],
function ($, _, Backbone, app) {
    var PostDetailsView = Backbone.Marionette.ItemView.extend({
        viewType: 'form',

        //className: 'post single',

        //template: 'PostIndex',

        events: {
            'click h3 a': 'showItem'
        },

        initialize: function (options) {
            if (!options.template) {
                this.template = 'PostIndex';
                this.className = 'post single';
            } else {
                this.template = options.template;
                this.className = 'post-details';
            }
        },

        serializeData: function () {
            var viewModel = null;

            if (this.template === 'PostIndex') {
                viewModel = {
                    Model:
                    {
                        Post: this.model.toViewJSON()
                    }
                };
            } else {
                viewModel = this.model.toViewJSON();
            }
            //json.Model.ShowThumbnails = this.model.get('Media').length > 1 ? true : false;
            return viewModel;
        },

        onShow: function () {
            this._showDetails();
        },

        showBootstrappedDetails: function () {
            this._showDetails();
        },

        _showDetails: function () {
        },

        refresh: function () {
        },

        showItem: function (e) {
            e.preventDefault();
            Backbone.history.navigate($(e.currentTarget).attr('href'), { trigger: true });
        }
    });

    return PostDetailsView;

});