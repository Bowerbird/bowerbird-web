/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// Observation
// -----------

define(['jquery', 'underscore', 'backbone', 'collections/observationmediacollection', 'models/observationmedia'],
function ($, _, Backbone, ObservationMediaCollection, ObservationMedia) {
    var Observation = Backbone.Model.extend({
        defaults: {
            Title: '',
            ObservedOn: null,
            Address: '',
            Latitude: null,
            Longitude: null,
            Category: '',
            AnonymiseLocation: false,
            ProjectIds: [],
            IsIdentificationRequired: false,
            Media: [],
            Comments: [],
            PrimaryMedia: null
        },

        urlRoot: '/observations',

        idAttribute: 'Id',

        initialize: function () {
            this.media = new ObservationMediaCollection();
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

        addMedia: function (mediaResource, description, licence) {
            var isPrimaryMedia = this.media.length === 0 ? true : false;
            this.media.add({ MediaResourceId: mediaResource.id, Description: description, Licence: licence, IsPrimaryMedia: isPrimaryMedia }, { mediaResource: mediaResource });
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
        }
    });

    return Observation;

});