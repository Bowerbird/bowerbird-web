/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />
/// <reference path="../../libs/moment/moment.js" />

// AccountUpdatePassword
// ---------------------

define(['jquery', 'underscore', 'backbone'],
function ($, _, Backbone) {

    var AccountUpdatePassword = Backbone.Model.extend({
        defaults: {
            Key: '',
            NewPassword: ''
        },

        url: function () {
            return '/account/updatepassword';
        },

        initialize: function () {
        },

        parse: function (resp, xhr) {
            return resp.Model.AccountUpdatePassword;
        }
    });

    return AccountUpdatePassword;

});