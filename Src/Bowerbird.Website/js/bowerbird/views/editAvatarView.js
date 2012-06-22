/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/icanhaz/icanhaz.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// EditAvatarView
// -------------------------

define(['jquery', 'underscore', 'backbone', 'app', 'ich', 'views/avataritemview', 'loadimage', 'models/mediaresource'], function ($, _, Backbone, app, ich, AvatarItemView, loadImage, MediaResource) {

    var EditAvatarView = Backbone.View.extend({

        id: 'avatar-fieldset',

        initialize: function (options) {
            _.extend(this, Backbone.Events);
            _.bindAll(this,
            'render',
            '_initMediaUploader',
            '_onUploadDone',
            '_onSubmitUpload',
            '_onUploadAdd'
            );
            this.model = options.model;
            this.currentUploadKey = 0;
            this.avatarItemView = null;
            log('EditAvatarView.Model is...:');
            log(this.model);
        },

        render: function () {
            log('editAvatarView:render');
            this._initMediaUploader();
            $('#avatar-viewer').append('<img src="' + this.model.get('Avatar').Files.medium.RelativeUri + '" />');
            return this;
        },

        _initMediaUploader: function () {
            $('#fileupload').fileupload({
                dataType: 'json',
                paramName: 'file',
                url: '/mediaresources/avatarupload',
                add: this._onUploadAd,
                submit: this._onSubmitUpload,
                done: this._onUploadDone,
                limitConcurrentUploads: 1
            });
        },

        _onUploadAdd: function (e, data) {
            log('editAvatarView:_onUploadAdd');
            this.currentUploadKey++;
            var mediaResource = new MediaResource({ Key: this.currentUploadKey.toString() });
            //this.model.setAvatar(mediaResource);
            var self = this;
            var tempImage = loadImage(
                data.files[0],
                function (img) {
                    if (img.type === "error") {
                        //log('Error loading image', img);
                    } else {
                        self.filesAdded++;
                        mediaResourceItemView.showTempMedia(img);
                        self._showMediaResourceItemView(self, mediaResourceItemView, $(img).width(), self.filesAdded === data.originalFiles.length);
                    }
                },
                { maxHeight: 220 }
            );

            if (!tempImage) {
                alert('No support for file/blob API!');
            }

            data.submit();
        },

        _onSubmitUpload: function (e, data) {
            data.formData = { Key: this.currentUploadKey, OriginalFileName: data.files[0].name };
        },

        _onUploadDone: function (e, data) {
            log('editAvatarView:_onUploadDone');
            this.model.set('AvatarId', data.result.Id);
            var mediaResource = new MediaResource(data.result);
            //this.$el.find('#avatar-viewer img').replaceWith($('<img src="' + mediaResource.get('ProfileImageUri') + '" alt="" />'));
            $('#avatar-viewer').empty().append('<img src="' + mediaResource.get('Files').medium.RelativeUri + '" width="200px;" />');
        }
    });

    return EditAvatarView;
});