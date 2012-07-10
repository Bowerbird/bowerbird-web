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
            TeamId: null,
            Type: 'Project'
        },

        idAttribute: 'Id',

        urlRoot: '/projects',

        setAvatar: function (mediaResource) {
            this.set('AvatarId', mediaResource.id);
        }
    });

    return Project;

});