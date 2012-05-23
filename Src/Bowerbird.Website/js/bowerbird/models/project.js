/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// Project
// -------

define(['jquery', 'underscore', 'backbone', 'app'], function ($, _, Backbone, app) {

    var Project = Backbone.Model.extend({
        defaults: {
            Id: '',
            Name: '',
            Description: '',
            Website: '',
            Avatar: null,
            Team: null,
            Type: 'Project'
        },

        idAttribute: 'Id',

        urlRoot: '/projects',

        toJSON: function () {
            return {
                Id: this.id,
                Name: this.get('Name'),
                Description: this.get('Description'),
                Website: this.get('Website'),
                Avatar: this.get('Avatar'), // TODO: Fix this to return id?
                Team: this.get('Team'),
                Type: 'Project'
            };
        },

        setAvatar: function (mediaResource) {
            this.set('Avatar', mediaResource.id);
        }
    });

    return Project;

});