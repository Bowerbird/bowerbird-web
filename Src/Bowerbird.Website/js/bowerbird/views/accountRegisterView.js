/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// AccountRegisterView
// -------------------

define(['jquery', 'underscore', 'backbone', 'app'], function ($, _, Backbone, app) {

    var AccountRegisterView = Backbone.Marionette.Layout.extend({
        className: 'register single',

        template: 'AccountRegister',
        
        initialize: function () {
        },

        showBootstrappedDetails: function () {
            this.initializeRegions();
        }
    });

    return AccountRegisterView;

}); 