/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// Organisation
// -------

define(['jquery', 'underscore', 'backbone'], function ($, _, Backbone) {

    var Organisation = Backbone.Model.extend({
        defaults: {
            Id: null,
            Name: '',
            Description: '',
            Website: '',
            TeamId: null,
            Type: 'Organisation'
        },

        idAttribute: 'Id',

        urlRoot: '/organisations',

        setAvatar: function (mediaResource) {
            this.set('AvatarId', mediaResource.id);
        }
    });

    return Organisation;

});