/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// FormView Base Class
// -------------------

define(['jquery', 'underscore', 'backbone', 'app'],
function ($, _, Backbone, app) {

    var FormView = Backbone.Marionette.Layout.extend({

        viewType: 'form',

        onShow: function () {
            this._showDetails();
        },

        showBootstrappedDetails: function () {
            this.initializeRegions();
            this._showDetails();
        },

        _showDetails: function () {
        },

        _cancel: function () {
            app.showPreviousContentView();
        }
    });

    return FormView;

});