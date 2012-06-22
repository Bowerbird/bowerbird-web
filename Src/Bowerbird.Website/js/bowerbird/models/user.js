/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// User
// ----

define(['jquery', 'underscore', 'backbone'], function ($, _, Backbone) {

    var User = Backbone.Model.extend({
        defaults: {
            IsTyping: false // Used by chat
        },

        idAttribute: 'Id'
    });

    return User;

});