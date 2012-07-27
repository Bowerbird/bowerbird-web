/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// Observation
// -----------

define(['jquery', 'underscore', 'backbone', 'collections/projectcollection', 'collections/mediaresourcecollection'],
function ($, _, Backbone, ProjectCollection, MediaResourceCollection) {
    var Observation = Backbone.Model.extend({
        defaults: {
            Title: '',
            ObservedOn: null,
            Address: '',
            Latitude: null,
            Longitude: null,
            Category: '',
            AnonymiseLocation: false,
            Projects: [],
            IsIdentificationRequired: false,
            Media: [],
            Comments: []
        },

        urlRoot: '/observations',

        idAttribute: 'Id',

        initialize: function (options) {
            this.mediaResources = new MediaResourceCollection();
            this.media = [];
        },

        toJSON: function () {
            return {
                Title: this.get('Title'),
                ObservedOn: this.get('ObservedOn'),
                Address: this.get('Address'),
                Latitude: this.get('Latitude'),
                Longitude: this.get('Longitude'),
                Category: this.get('Category'),
                AnonymiseLocation: this.get('AnonymiseLocation'),
                Projects: this.get('Projects'),
                IsIdentificationRequired: this.get('IsIdentificationRequired'),
                Media: this.get('Media'),
                Comments: this.get('Comments')
            };
        },

        //        toViewJSON: function () {
        //            // returns JSON containing all data for view
        //            return Backbone.Model.prototype.toJSON.apply(this, arguments);
        //        },

        addProject: function (id) {
            var projects = this.get('Projects');
            projects.push(id);
            this.set('Projects', projects);
        },

        removeProject: function (id) {
            var projects = this.get('Projects');
            this.set('Projects', _.without(projects, id));
        },

        //        addMediaResource: function (mediaResource) {
        //            mediaResource.on('change', this._setMedia, this);
        //            this.mediaResources.add(mediaResource);
        //            this._setMedia();
        //        },

        //        removeMediaResource: function (id) {
        //            this.mediaResources.remove(this.mediaResources.get(id));
        //            this._setMedia();
        //        },

        //        _setMedia: function () {
        //            var media = this.mediaResources.map(function (mediaResource) {
        //                return { MediaResourceId: mediaResource.id, Description: 'description goes here...', Licence: 'licence goes here...' };
        //            });
        //            this.set('Media', media);
        //        }

        addMedia: function (mediaResource, description, licence) {
            this.mediaResources.add(mediaResource);
            this.media.push({ mediaResource: mediaResource, description: description, licence: licence });
            this._setMedia();
        },

        updateMedia: function (id, description, licence) {
            var mediaResource = this.mediaResources.get(id);
            this.media = _.filter(this.media, function (item) {
                return item.mediaResource.id !== id;
            });
            this.media.push({ mediaResource: mediaResource, description: description, licence: licence });
            this._setMedia();
        },

        removeMedia: function (id) {
            var mediaResource = this.mediaResources.get(id);
            this.mediaResources.remove(mediaResource);
            this.media = _.filter(this.media, function (item) {
                return item.mediaResource.id !== id;
            });
            this._setMedia();
        },

        _setMedia: function () {
            var media = this.media.map(function (item) {
                return { MediaResourceId: item.mediaResource.id, Description: item.description, Licence: item.licence };
            });
            this.set('Media', media);
        }
    });

    return Observation;

});