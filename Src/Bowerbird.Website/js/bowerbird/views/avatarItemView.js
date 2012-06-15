/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// AvatarItemView
// -------------------------

define(['jquery', 'underscore', 'backbone', 'app', 'ich'], function ($, _, Backbone, app, ich) {

    var AvatarItemView = Backbone.View.extend({

        className: 'avatar-uploaded',

        events: {
            'click .view-media-resource-button': 'viewMediaResource',
            'click .add-caption-button': 'viewMediaResource',
            'click .remove-media-resource-button': 'removeMediaResource'
        },

        //template: "AvatarUploaded",

        initialize: function (options) {
            _.extend(this, Backbone.Events);
            _.bindAll(this,
            'showTempMedia',
            '_showUploadedMedia'
            );
            this.mediaResource = options.model;
            log('avatarItemView.initialize: mediaResource >');
            log(this.mediaResource);
            //this.mediaResource.on('change:mediumImageUri', this.showUploadedMedia);
        },

        render: function () {
            log('avatarItemView:render');
            //            this.$el.append(ich.AvatarUploaded(this.MediaResource.toJSON()));
            //            return this;
            this._showUploadedMedia();
        },

        viewMediaResource: function () {
            alert('Coming soon');
        },

        removeMediaResource: function () {
            this.remove();
            $('#avatar-add-pane').show();
        },

        showTempMedia: function (img) {
            log('avatarItemView:_showTempMedia');
            this.$el.find('div:first-child img').replaceWith($(img));
        },

        _showUploadedMedia: function (mediaResource) {
            log('avatarItemView:_showUploadedMedia');
            this.$el.find('div:first-child img').replaceWith($('<img src="' + this.mediaResource.get('MediumImageUri') + '" alt="" />'));
        }
    });

    return AvatarItemView;
});