/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// Observation
// -----------

define(['jquery', 'underscore', 'backbone', 'app', 'collections/projectcollection', 'collections/mediaresourcecollection'], function ($, _, Backbone, app, ProjectCollection, MediaResourceCollection) {

    //    var getValue = function(object, prop) {
    //        if (!(object && object[prop])) return null;
    //        return _.isFunction(object[prop]) ? object[prop]() : object[prop];
    //    };

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

        //        url: function () {
        //            var base = getValue(this, 'urlRoot') || getValue(this.collection, 'url') || urlError();
        //            if (this.isNew()) return base;
        //            return base + (base.charAt(base.length - 1) == '/' ? '' : '/') + encodeURIComponent(this.id);
        //        },

        //        url: function () {
        //            if (this.isNew()) {
        //                Backbone.Model.prototype.url.call(this);
        //            } else {
        //                var base = getValue(this, 'urlRoot') || getValue(this.collection, 'url') || urlError();
        //                if (this.isNew()) return base;
        //                return base + (base.charAt(base.length - 1) == '/' ? '' : '/') + encodeURIComponent(this.id);
        //                if (this.id) {
        //                    return this.id.split('/')[1];
        //                }
        //                return ' ';
        //            }
        //        },

        urlRoot: '/observations',

        idAttribute: 'Id',

        initialize: function (options) {
            //this.projects = new ProjectCollection();
            this.mediaResources = new MediaResourceCollection();

            //            this.addMediaResources = new MediaResourceCollection();
            //            this.mediaResources = new MediaResourceCollection();
            //            this.removeMediaResources = new MediaResourceCollection();
            //            if (_.has(options, 'mediaResources')) {
            //                this.mediaResources.reset(options.mediaResources);
            //            }
        },

        //        allMediaResources: function () {
        //            return new MediaResourceCollection(this.addMediaResources.models).add(this.mediaResources.models).toArray();
        //        },

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
                //                AddMedia: this.addMediaResources.map(function (mediaResource) {
                //                    return { MediaResourceId: mediaResource.id, Description: 'stuff', Licence: 'licenceX' };
                //                }),
                //                Media: this.mediaResources.map(function (mediaResource) {
                //                    return { MediaResourceId: mediaResource.id, Description: 'stuff', Licence: 'licenceX' };
                //                }),
                //                RemoveMedia: this.removeMediaResources.pluck('id')
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