/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ObservationDetailsView
// ----------------------

define(['jquery', 'underscore', 'backbone', 'app'], function ($, _, Backbone, app) {

    var ObservationDetailsView = Backbone.Marionette.View.extend({
        template: 'ObservationDetails',

        onShow: function () {
            this._showDetails();
        },

        showBootstrappedDetails: function () {
            //this.$el = $('#content .observation');
        },

        _showDetails: function () {
        }
    });

    return ObservationDetailsView;

});