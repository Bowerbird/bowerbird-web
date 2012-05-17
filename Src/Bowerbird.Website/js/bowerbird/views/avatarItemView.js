/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// AvatarItemView
// -------------------------

define(['jquery', 'underscore', 'backbone', 'app', 'ich'], function ($, _, Backbone, app, ich) {

    AvatarItemView = Backbone.View.extend({

        className: 'avatar-uploaded',

        events: {
            'click .view-media-resource-button': 'viewMediaResource',
            'click .add-caption-button': 'viewMediaResource',
            'click .remove-media-resource-button': 'removeMediaResource'
        },

        template: "AvatarUploaded",

        initialize: function (options) {
            _.extend(this, Backbone.Events);
            _.bindAll(this, 'showTempMedia', 'showUploadedMedia');
            this.mediaResource = options.MediaResource;
            this.mediaResource.on('change:mediumImageUri', this.showUploadedMedia);
        },

        onRender: function () {
//            this.$el.append(ich.AvatarUploaded(this.MediaResource.toJSON()));
//            return this;
        },

        viewMediaResource: function () {
            alert('Coming soon');
        },

        removeMediaResource: function () {
            this.remove();
            $('#avatar-add-pane').show();
        },

        showTempMedia: function (img) {
            this.$el.find('div:first-child img').replaceWith($(img));
        },

        showUploadedMedia: function (mediaResource) {
            this.$el.find('div:first-child img').replaceWith($('<img src="' + mediaResource.get('MediumImageUri') + '" alt="" />'));
        }
    });

    return AvatarItemView;
});