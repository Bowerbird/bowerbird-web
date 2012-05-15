/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// MediaResourceItemView
// ---------------------

define(['jquery', 'underscore', 'backbone', 'app', 'ich'], function ($, _, Backbone, app, ich) {

    var MediaResourceItemView = Backbone.View.extend({
        className: 'media-resource-uploaded',

        events: {
            'click .view-media-resource-button': 'viewMediaResource',
            'click .add-caption-button': 'viewMediaResource',
            'click .remove-media-resource-button': 'removeMediaResource'
        },

        initialize: function (options) {
            _.extend(this, Backbone.Events);
            _.bindAll(this, 'showTempMedia', 'showUploadedMedia', 'removeMediaResource');
            this.model.on('change:MediumImageUri', this.showUploadedMedia);
        },

        render: function () {
            this.$el.append(ich.ObservationMediaResourceUploaded(this.model.toJSON())).css({ position: 'absolute', top: '-250px' });
            return this;
        },

        viewMediaResource: function () {
            alert('Coming soon');
        },

        removeMediaResource: function () {
            this.trigger('mediaresourceviewremove', this.model, this);
            //            var addToRemoveList = false;
            //            if (app.get('newObservation').mediaResources.find(function (mr) { return mr.id == this.model.id; }) != null) {
            //                addToRemoveList = true;
            //            }
            //            app.get('newObservation').addMediaResources.remove(this.model.id);
            //            app.get('newObservation').mediaResources.remove(this.model.id);
            //            if (addToRemoveList) {
            //                app.get('newObservation').removeMediaResources.add(this.model);
            //            }
            //this.remove();
        },

        showTempMedia: function (img) {
            var $image = $(img);
            this.$el.find('div:first-child img').replaceWith($image);
            this.$el.width($image.width());
            this.imageWidth = $image.width();
        },

        showUploadedMedia: function (mediaResource) {
            log(mediaResource);
            this.$el.find('div:first-child img').replaceWith($('<img src="' + mediaResource.get('MediumImageUri') + '" alt="" />'));
        }
    });

    return MediaResourceItemView;

});