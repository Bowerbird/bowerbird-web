/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />
/// <reference path="../../libs/moment/moment.js" />

// AccountUpdate
// -------------

define(['jquery', 'underscore', 'backbone'],
function ($, _, Backbone) {

    var AccountUpdate = Backbone.Model.extend({
        defaults: {
            Id: null,
            Name: '',
            Email: '',
            AvatarId: null,
            BackgroundId: null
        },

        idAttribute: 'Id',

        url: function () {
            return '/account/update';
        },

        initialize: function () {
        },

        parse: function (resp, xhr) {
            return resp.Model.User;
        },

        setAvatar: function (mediaResource) {
            this.set('AvatarId', mediaResource.id);
        }
    });

    return AccountUpdate;

});