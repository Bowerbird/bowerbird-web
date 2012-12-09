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
            '_onImageUploadAdd'
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
            this.$el.find('#avatar-viewer').empty().append('<img src="' + this.model.get('Avatar').Image.Square200.Uri + '" />');
            return this;
        },

        _initMediaUploader: function () {
            $('#file').fileupload({
                dataType: 'json',
                paramName: 'file',
                url: '/mediaresources',
                add: this._onImageUploadAdd
            });
        },

        _onImageUploadAdd: function (e, data) {
            this.$el.find('#avatar-viewer').empty().append('<img class="progress-indicator" src="/img/loaderx.gif" alt="" style="width: " />');

            this.key = app.generateGuid();

            data.formData = { Key: this.key, FileName: data.files[0].name, Type: 'file', Usage: 'avatar' };
            if (window.isIEFail) {
                data.formData.ie = true;
            }

            data.submit();
        },

        _onMediaResourceUploadSuccess: function (data) {
            if (this.key === data.Key) {
                var mediaResource = new MediaResource(data);
                this.model.avatar = mediaResource;
                this.model.set('AvatarId', mediaResource.id);

                this.$el.find('#avatar-viewer').empty().append('<img src="' + mediaResource.get('Image').Square200.Uri + '" alt="" />');
            }
        },

        _onMediaResourceUploadFailure: function (key, reason) {
        },

        onClose: function () {
            app.vent.off('mediaresourceuploadsuccess', this._onMediaResourceUploadSuccess, this);
            app.vent.off('mediaresourceuploadfailure', this._onMediaResourceUploadFailure, this);
        }
    });

    return EditAvatarView;
});