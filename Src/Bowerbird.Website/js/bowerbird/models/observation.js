/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// Observation
// -----------

define(['jquery', 'underscore', 'backbone', 'collections/projectcollection', 'collections/mediaresourcecollection'],
function ($, _, Backbone, ProjectCollection, MediaResourceCollection) 
{
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
            Media: []
        },

        urlRoot: '/observations',

        idAttribute: 'Id',

        initialize: function (options) {
            this.mediaResources = new MediaResourceCollection();
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
                Media: this.get('Media')
            };
        },

        addProject: function (id) {
            var projects = this.get('Projects');
            projects.push(id);
            this.set('Projects', projects);
        },

        removeProject: function (id) {
            var projects = this.get('Projects');
            this.set('Projects', _.without(projects, id));
        },

        addMediaResource: function (mediaResource) {
            mediaResource.on('change', this._setMedia, this);
            this.mediaResources.add(mediaResource);
            this._setMedia();
        },

        removeMediaResource: function (id) {
            this.mediaResources.remove(this.mediaResources.get(id));
            this._setMedia();
        },

        _setMedia: function () {
            var media = this.mediaResources.map(function (mediaResource) {
                return { MediaResourceId: mediaResource.id, Description: 'stuff', Licence: 'licenceX' }
            });
            this.set('Media', media);
        }
    });

    return Observation;

});