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
            FirstName: '',
            LastName: '',
            Email: '',
            AvatarId: null,
            Type: 'User'
        },

        idAttribute: 'Id',

        urlRoot: '/users',

        setAvatar: function (mediaResource) {
            this.set('AvatarId', mediaResource.id);
        }
    });

    return User;

});