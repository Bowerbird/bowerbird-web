/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// Login
// -----

define(['jquery', 'underscore', 'backbone'], function ($, _, Backbone) {

    var Login = Backbone.Model.extend({
        defaults: {
            Email: '',
            Password: '',
            ReturnUrl: '/',
            RememberMe: false
        },
        
        url: '/account/login'
    });

    return Login;

});