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

    EditAvatarView = Backbone.View.extend({

        id: 'avatar-fieldset',

        //        events: {
        //            'click .remove-media-resource-button': 'removeMediaResource'
        //        },

        initialize: function (options) {
            _.extend(this, Backbone.Events);
            _.bindAll(this,
            'render',
            '_initMediaUploader',
            '_onUploadDone',
            '_onSubmitUpload',
            '_onUploadAdd'//,
            //'removeMediaResource'
            );
            this.model = options.model;
            this.currentUploadKey = 0;
            this.avatarItemView = null;
        },

        render: function () {
            log('editAvatarView:render');
            this._initMediaUploader();
            $('#avatar-viewer').append('<img src="' + this.model.get('Avatar').UrlToImage + '" />');
            return this;
        },

        //        onShow: function () {
        //            
        //        },

        //        _showDetails: function () {
        //            //ich.AvatarChooseFile().appendTo($('#avatar-add-pane'));
        //            this._initMediaUploader();
        //            //return this;
        //        },

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
            this.model.setAvatar(mediaResource);

            //this.avatarItemView = new AvatarItemView({ model: mediaResource });
            //avatarItemView.on('avataritemview:remove', this._removeMediaResource);
            //this.mediaResourceItemViews.push(mediaResourceItemView);
            //$('#media-resource-add-pane').before(mediaResourceItemView.render().el);

            //$('#avatar-viewer').append(this.avatarItemView.render().el);

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
            var self = this;
            this.group.set('Avatar', data.result);
            this.currentUploadKey++;
            var mediaResource = new Bowerbird.Models.MediaResource({ Key: self.CurrentUploadKey });
            this.avatarItemView = new Bowerbird.Views.AvatarItemView({ mediaResource: mediaResource });
            $('#avatar-add-pane').hide();
            $('#avatar-viewer').append(this.avatarItemView.render().el);
            loadImage(
                data.files[0],
                function (img) {
                    if (img instanceof HTMLImageElement) { // FF seems to fire this handler twice, on second time returning error, which we ignore :(
                        self.avatarItemView.showTempMedia(img);
                        $('#media-resource-avatar').animate({ scrollLeft: 100000 });
                    }
                },
                {
                    maxHeight: 220
                }
            );
        },

        _removeMediaResource: function () {
            this.group.set('Avatar', null);
            this.avatarItemView = null;
            this.$el.find('#avatar-add-pane').append(ich.AvatarChooseFile());
            this._initMediaUploader();
        }
    });

    return EditAvatarView;
});