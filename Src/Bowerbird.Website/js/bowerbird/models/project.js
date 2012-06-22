/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// Project
// -------

define(['jquery', 'underscore', 'backbone'], function ($, _, Backbone) {

    var Project = Backbone.Model.extend({
        defaults: {
            Name: '',
            Description: '',
            Website: '',
            AvatarId: null,
            Team: null,
            Type: 'Project'
        },

        idAttribute: 'Id',

        urlRoot: '/projects',

//        toJSON: function () {
//            return {
//                Name: this.get('Name'),
//                Description: this.get('Description'),
//                Website: this.get('Website'),
//                Avatar: this.get('Avatar'), // TODO: Fix this to return id?
//                Team: this.get('Team'),
//                Type: 'Project'
//            };
//        },

        setAvatar: function (mediaResource) {
            this.set('AvatarId', mediaResource.id);
        }
    });

    return Project;

});