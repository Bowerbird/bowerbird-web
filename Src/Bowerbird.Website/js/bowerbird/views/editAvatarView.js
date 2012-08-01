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
            '_initMediaUploader'
            //            '_onUploadDone',
            //            '_onSubmitUpload',
            //            '_onUploadAdd'
            );
            this.model = options.model;
            this.currentUploadKey = 0;
            this.avatarItemView = null;

            app.vent.on('mediaresourceuploadsuccess', this._onMediaResourceUploadSuccess, this);
            app.vent.on('mediaresourceuploadfailure', this._onMediaResourceUploadFailure, this);
        },

        render: function () {
            this._initMediaUploader();
            $('#avatar-viewer').append('<img src="' + this.model.get('Avatar').Image.Square200.RelativeUri + '" />');
            return this;
        },

        _initMediaUploader: function () {
            $('#fileupload').fileupload({
                dataType: 'json',
                paramName: 'file',
                url: '/mediaresources',
                add: this._onImageUploadAdd

                //                dataType: 'json',
                //                paramName: 'file',
                //                url: '/mediaresources',
                //                add: this._onUploadAd,
                //                submit: this._onSubmitUpload,
                //                done: this._onUploadDone,
                //                limitConcurrentUploads: 1
            });
        },

        _onImageUploadAdd: function (e, data) {
            this.key = app.generateGuid();

            data.formData = { Key: this.key, OriginalFileName: data.files[0].name, MediaType: '', Usage: 'avatar' };
            if (window.isIEFail) {
                data.formData.ie = true;
            }

            data.submit();
        },

        _onMediaResourceUploadSuccess: function (data) {
            //            var mediaResource = this.currentUploads.find(function (item) {
            //                return item.get('Key') === data.Key;
            //            });
            //            mediaResource.set(data);
            var mediaResource = new MediaResource(data);
            log('here', mediaResource);
            this.model.avatar = mediaResource;
            this.model.set('AvatarId', mediaResource.id);
            //this.model.addMedia(mediaResource, '', app.authenticatedUser.defaultLicence);
            //this._updateProgress();
            $('#avatar-viewer').empty().append('<img src="' + mediaResource.get('Image').Square200.RelativeUri + '" alt="" />');
        },

        _onMediaResourceUploadFailure: function (key, reason) {
            //            var mediaResource = this.currentUploads.find(function (item) {
            //                return item.get('Key') === key;
            //            });
            //            this.currentUploads.remove(mediaResource);
            //            this.failedUploads.add(mediaResource);
            //            this._updateProgress();
        },


        //        _onUploadAdd: function (e, data) {
        //            log('editAvatarView:_onUploadAdd');
        //            this.currentUploadKey++;
        //            var mediaResource = new MediaResource({ Key: this.currentUploadKey.toString() });
        //            //this.model.setAvatar(mediaResource);
        //            var self = this;
        //            var tempImage = loadImage(
        //                data.files[0],
        //                function (img) {
        //                    if (img.type === "error") {
        //                        //log('Error loading image', img);
        //                    } else {
        //                        self.filesAdded++;
        //                        mediaResourceItemView.showTempMedia(img);
        //                        self._showMediaResourceItemView(self, mediaResourceItemView, $(img).width(), self.filesAdded === data.originalFiles.length);
        //                    }
        //                },
        //                { maxHeight: 220 }
        //            );

        //            if (!tempImage) {
        //                alert('No support for file/blob API!');
        //            }

        //            data.submit();
        //        },

        //        _onSubmitUpload: function (e, data) {
        //            data.formData = { Key: this.currentUploadKey, OriginalFileName: data.files[0].name };
        //        },

        //        _onUploadDone: function (e, data) {
        //            log('editAvatarView:_onUploadDone');
        //            this.model.set('AvatarId', data.result.Id);
        //            var mediaResource = new MediaResource(data.result);
        //            //this.$el.find('#avatar-viewer img').replaceWith($('<img src="' + mediaResource.get('ProfileImageUri') + '" alt="" />'));
        //            $('#avatar-viewer').empty().append('<img src="' + mediaResource.get('Files').ThumbnailMedium.RelativeUri + '" width="200px;" />');
        //        }
        onClose: function () {
            app.vent.off('mediaresourceuploadsuccess', this._onMediaResourceUploadSuccess, this);
            app.vent.off('mediaresourceuploadfailure', this._onMediaResourceUploadFailure, this);
        }
    });

    return EditAvatarView;
});