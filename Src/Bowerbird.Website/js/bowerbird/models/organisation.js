/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// Organisation
// ------------

define(['jquery', 'underscore', 'backbone'], function ($, _, Backbone) {

    var Organisation = Backbone.Model.extend({
        defaults: {
            Name: '',
            Description: '',
            Website: '',
            Avatar: null,
            Type: 'Organisation'
        },

        idAttribute: 'Id',

        urlRoot: '/organisations',

        toJSON: function () {
            return {
                Name: this.get('Name'),
                Description: this.get('Description'),
                Website: this.get('Website'),
                Avatar: this.get('Avatar'), // TODO: Fix this to return id?
                Type: 'Organisation'
            };
        },

        setAvatar: function (mediaResource) {
            this.set('Avatar', mediaResource.id);
        }
    });

    return Organisation;

});