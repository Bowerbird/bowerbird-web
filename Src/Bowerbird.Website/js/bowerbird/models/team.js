/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// Team
// -------

define(['jquery', 'underscore', 'backbone', 'app'], function ($, _, Backbone, app) {

    var Team = Backbone.Model.extend({
        defaults: {
            Type: 'Team',
        },

        idAttribute: 'Id',

        url: '/teams/',

        toJSON: function () {
            return {
                Id: this.id,
                Name: this.get('Name'),
                Description: this.get('Description'),
                Website: this.get('Website'),
                Avatar: this.get('Avatar'), // TODO: Fix this to return id?
                Organisation: this.get('Organisation'),
                Type: 'Team'
            };
        }
    });

    return Team;

});