/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// Observation
// -----------

define(['jquery', 'underscore', 'backbone', 'collections/observationmediacollection', 'models/mediaresource'],
function ($, _, Backbone, ObservationMediaCollection, MediaResource) {
    var Observation = Backbone.Model.extend({
        defaults: {
            Id: null,
            Title: '',
            ObservedOn: null,
            Address: '',
            Latitude: null,
            Longitude: null,
            Category: '',
            AnonymiseLocation: false,
            ProjectIds: [],
            Media: []
        },

        url: function () {
            url = '/';
            if (this.id) {
                url += this.id;
            } else {
                url += 'observations';
            }
            return url;
        },

        idAttribute: 'Id',

        initialize: function () {
            this.media = new ObservationMediaCollection();

            if (this.id) {
                var newMedia = [];
                var tempExistingMedia = this.get('Media');
                _.each(tempExistingMedia, function (item) {
                    item.MediaResourceId = item.MediaResource.Id;
                    newMedia.push(item);
                    var mediaResource = new MediaResource(item.MediaResource);
                    this.addMedia(mediaResource, item.Description, item.Licence, item.IsPrimaryMedia);
                }, this);
                this.set('Media', newMedia);

                var projectIds = [];
                _.each(this.get('Projects'), function (proj) {
                    projectIds.push(proj.Id);
                }, this);

                this.set('ProjectIds', projectIds);
            }

            this.media.on('add', this.onMediaChange, this);
            this.media.on('change', this.onMediaChange, this);
            this.media.on('remove', this.onMediaChange, this);
            this.media.on('change:IsPrimaryMedia', this.onPrimaryMediaChange, this);
        },

        addProject: function (id) {
            var projects = this.get('ProjectIds');
            projects.push(id);
            this.set('ProjectIds', projects);
        },

        removeProject: function (id) {
            var projects = this.get('ProjectIds');
            this.set('ProjectIds', _.without(projects, id));
        },

        addMedia: function (mediaResource, description, licence, isPrimaryMedia) {
            if (isPrimaryMedia == null) {
                isPrimaryMedia = this.media.length === 0 ? true : false;
            }
            this.media.add({ MediaResourceId: mediaResource.get('Id'), Description: description, Licence: licence, IsPrimaryMedia: isPrimaryMedia }, { mediaResource: mediaResource });
        },

        removeMedia: function (media) {
            this.media.remove(media);

            if (media.get('IsPrimaryMedia') === true && this.media.length > 0) {
                this.setPrimaryMedia(this.media.first());
            }
        },

        onMediaChange: function (media) {
            var allMedia = this.media.map(function (item) {
                return item.toJSON();
            });
            this.set('Media', allMedia);
        },

        setPrimaryMedia: function (media) {
            this.media.each(function (item) {
                if (item.get('MediaResourceId') === media.get('MediaResourceId')) {
                    item.set('IsPrimaryMedia', true);
                } else {
                    item.set('IsPrimaryMedia', false);
                }
            }, this);
        },

        setIdentification: function (identification) {
            this.set('NewSighting', true); // Forces saving of this id
            this.set('IdentificationComments', identification.get('IdentificationComments'));
            this.set('IsCustomIdentification', identification.get('IsCustomIdentification'));
            this.set('Taxonomy', identification.get('Taxonomy'));
        },

        setSightingNote: function (sightingNote) {
            this.set('NewSighting', true); // Forces saving of this note
            this.set('NoteComments', sightingNote.get('NoteComments'));
            this.set('Descriptions', sightingNote.get('Descriptions'));
            this.set('Tags', sightingNote.get('Tags'));
        },

        hasLatLong: function () {
            return this.get('Latitude') !== null && this.get('Longitude') !== null && this.get('Latitude') !== '' && this.get('Longitude') !== null;
        }
    });

    return Observation;

});