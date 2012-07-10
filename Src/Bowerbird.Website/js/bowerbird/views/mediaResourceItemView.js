/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// MediaResourceItemView
// ---------------------

define(['jquery', 'underscore', 'backbone', 'app', 'ich'],
function ($, _, Backbone, app, ich) {
    var MediaResourceItemView = Backbone.View.extend({
        className: 'media-resource-uploaded',

        events: {
            'click .view-media-resource-button': 'viewMediaResource',
            'click .add-caption-button': 'viewMediaResource',
            'click .remove-media-resource-button': 'removeMediaResource'
        },

        initialize: function (options) {
            _.extend(this, Backbone.Events);
            _.bindAll(this,
            'showTempImageMedia',
            'showUploadedImageMedia',
            'removeMediaResource');

            // if image or video... 
            this.model.on('change:Files', this.showUploadedImageMedia);
        },

        render: function () {
            this.$el.append(ich.ObservationMediaResourceUploaded(this.model.toJSON())).css({ position: 'absolute', top: '-250px' });
            return this;
        },

        viewMediaResource: function () {
            alert('Coming soon');
        },

        removeMediaResource: function () {
            this.trigger('mediaresourceview:remove', this.model, this);
        },

        showTempImageMedia: function (img) {
            var $image = $(img);
            this.$el.find('div:first-child img').replaceWith($image);
            this.$el.width($image.width());
            this.imageWidth = $image.width();
        },

        showUploadedImageMedia: function (mediaResource) {
            log('MediaResourceItemView.showUploadedMedia', mediaResource);
            this.$el.find('div:first-child img').replaceWith($('<img src="' + mediaResource.get('Files').FullMedium.RelativeUri + '" alt="" />'));
        },

        showVideoMedia: function (mediaResource) {
            var src = mediaResource.get('Preview');
            log('MediaResourceItemView.showVideoMedia:', src);
            this.$el.find('div:first-child').replaceWith(ich.VideoPreview({ Width: 300, Height: 220, Source: src }));
            //this.$el.append(ich.VideoPreview({ Width: 220, Height: 200, Source: src }));
        }
    });

    return MediaResourceItemView;

});