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
            Id: null,
            Name: '',
            Description: '',
            Website: '',
            Type: 'Project',
            Categories: []
        },

        idAttribute: 'Id',

        urlRoot: '/projects',

        setAvatar: function (mediaResource) {
            this.set('AvatarId', mediaResource.id);
        },
        
        addCategory: function (id) {
            var categories = this.get('Categories');
            categories.push(id);
            this.set('Categories', categories);
        },

        removeCategory: function (id) {
            var categories = this.get('Categories');
            this.set('Categories', _.without(categories, id));
        }
    });

    return Project;

});