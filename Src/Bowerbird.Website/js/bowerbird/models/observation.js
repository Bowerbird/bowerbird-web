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
            this.set('Identification', identification.toJSON());
        },

        setSightingNote: function (sightingNote) {
            this.set('Note', sightingNote.toJSON());
        },

        hasLatLong: function () {
            return this.get('Latitude') !== null && this.get('Longitude') !== null && this.get('Latitude') !== '' && this.get('Longitude') !== null;
        },

        isValid: function (fireEvent) {
            var isValid = true;
            var errors = [];

            var title = this.get('Title') != null ? this.get('Title').trim() : '';
            var category = this.get('Category');
            var media = this.get('Media');

            if (!this.hasLatLong()) {
                errors.push({ Field: 'Location', Message: 'Please enter a location. Either drag and drop the pin or enter an exact coordinate (click the options button).' });
                isValid = false;
            }

            if (title.length == 0) {
                errors.push({ Field: 'Title', Message: 'Please enter a title.' });  
                isValid = false;
            }

            if (category.length == 0) {
                errors.push({ Field: 'Category', Message: 'Please select a category.' });
                isValid = false;
            }

            if (media.length == 0) {
                errors.push({ Field: 'Media', Message: 'Please add at least one media file.' });
                isValid = false;
            }

            if (fireEvent === true) {
                this.trigger('validated', this, errors);
            }

            return isValid;
        }
    });

    return Observation;

});