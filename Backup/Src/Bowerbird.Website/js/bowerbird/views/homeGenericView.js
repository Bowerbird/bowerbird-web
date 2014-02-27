/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// HomeGenericView
// ---------------

define(['jquery', 'underscore', 'backbone', 'ich', 'app'], function ($, _, Backbone, ich, app) {

    var HomeGenericView = Backbone.Marionette.Layout.extend({
        viewType: 'form',

        className: '',

        template: 'HomeAbout',

        initialize: function(options) {
        },

        serializeData: function() {
            return {
                Model: {
                    
                }
            };
        },

        onShow: function() {
            this._showDetails();
        },

        showBootstrappedDetails: function() {
            this.initializeRegions();
            this._showDetails();
        },

        _showDetails: function() {
        }
    });

    return HomeGenericView;

}); 