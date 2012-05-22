/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// Organisation
// -------

define(['jquery', 'underscore', 'backbone', 'app'], function ($, _, Backbone, app) {

    var Organisation = Backbone.Model.extend({
        defaults: {
            Type: 'Organisation',
            Avatar: {
                Id: '',
                Url: '',
                AltTag: ''
            }
        },

        idAttribute: 'Id',

        url: '/organisations/',

        toJSON: function () {
            return {
                Id: this.id,
                Name: this.get('Name'),
                Description: this.get('Description'),
                Website: this.get('Website'),
                Avatar: this.get('Avatar'), // TODO: Fix this to return id?
                Type: 'Organisation'
            };
        }
    });

    return Organisation;

});