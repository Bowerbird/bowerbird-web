/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/icanhaz/icanhaz.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// BackgroundImageFormView
// -----------------------

define(['jquery', 'underscore', 'backbone', 'app', 'ich', 'loadimage', 'models/mediaresource'], function ($, _, Backbone, app, ich, loadImage, MediaResource) {

    var BackgroundImageFormView = Backbone.View.extend({

        id: 'background-fieldset',

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

            app.vent.on('mediaresourceuploadsuccess', this._onMediaResourceUploadSuccess, this);
            app.vent.on('mediaresourceuploadfailure', this._onMediaResourceUploadFailure, this);
        },

        render: function () {
            this._initMediaUploader();
            //$('#background-viewer').empty().append('<img src="' + this.model.get('Background').Image.Large.Uri + '" alt="" />');
            //$('#background-viewer').empty().append('<img src="/img/project-background.png" alt="" />');
            return this;
        },

        _initMediaUploader: function () {
            this.$el.find('#file').fileupload({
                dataType: 'json',
                paramName: 'file',
                url: '/mediaresources',
                add: this._onImageUploadAdd
            });
        },

        _onImageUploadAdd: function (e, data) {
            this.$el.find('#background-viewer').empty().append('<img class="progress-indicator" src="/img/loader-small.gif" alt="" style="width: " />');

            this.key = app.generateGuid();

            data.formData = { Key: this.key, FileName: data.files[0].name, Type: 'file', Usage: 'background' };
            if (window.isIEFail) {
                data.formData.ie = true;
            }

            data.submit();
        },

        _onMediaResourceUploadSuccess: function (data) {
            if (this.key === data.Key) {
                var mediaResource = new MediaResource(data);
                this.model.background = mediaResource;
                this.model.set('BackgroundId', mediaResource.id);

                this.$el.find('#background-viewer').empty().append('<img src="' + mediaResource.get('Image').Large.Uri + '" alt="" />');
            }
        },

        _onMediaResourceUploadFailure: function (key, reason) {
        },

        onClose: function () {
            app.vent.off('mediaresourceuploadsuccess', this._onMediaResourceUploadSuccess, this);
            app.vent.off('mediaresourceuploadfailure', this._onMediaResourceUploadFailure, this);
        }
    });

    return BackgroundImageFormView;
});