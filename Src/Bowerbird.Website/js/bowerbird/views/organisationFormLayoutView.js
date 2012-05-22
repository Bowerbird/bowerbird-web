/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ProjectFormLayoutView
// -------------------------

define(['jquery', 'underscore', 'backbone', 'app', 'ich', 'multiselect', 'loadimage', 'fileupload'], function ($, _, Backbone, app, ich, loadImage) {

    var OrganisationFormLayoutView = Backbone.Marionette.Layout.extend({

        className: 'form single-medium organisation-create-form',

        tempalte: 'OrganisationForm',

        regions: {
            avatar: '#avatar-fieldset'
        },

        events: {
            'click #cancel': '_cancel',
            'click #save': '_save',
            'change input#name': '_contentChanged',
            'change textarea#description': '_contentChanged',
            'change input#website': '_contentChanged'
        },

        serializeData: function () {
            return {
                Model: {
                    Organisations: this.model.toJSON()
                }
            };
        },

        onShow: function () {
            this._showDetails();
            this._initMediaUploader();
        },

        showBootstrappedDetails: function () {
            this.initializeRegions();
            this._showDetails();
        },

        _initMediaUploader: function () {
            $('#fileupload').fileupload({
                dataType: 'json',
                paramName: 'file',
                url: '/mediaresource/avatarupload',
                add: this._onUploadAdd,
                submit: this._onSubmitUpload,
                done: this._onUploadDone,
                limitConcurrentUploads: 1
            });
        },

        _onUploadAdd: function (e, data) {
            var self = this;
            var tempImage = loadImage(
                data.files[0],
                function (img) {
                    if (img.type === "error") {
                        log('Error loading image', img);
                    } else {
                        this._showTempMedia(img);
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
            var mediaResource = this.model.mediaResources.find(function (item) {
                return item.get('Key') === data.result.Key;
            });
            mediaResource.set(data.result);
            this._showUploadedMedia(mediaResource);
        },

        _showTempMedia: function (img) {
            var $image = $(img);
            this.$el.find('#avatar-field img').replaceWith($image);
            this.$el.width($image.width());
            this.imageWidth = $image.width();
        },

        _showUploadedMedia: function (mediaResource) {
            this.$el.find('#avatar-field img').replaceWith($('<img src="' + mediaResource.get('MediumImageUri') + '" alt="" />'));
        },

        _contentChanged: function (e) {
            var target = $(e.currentTarget);
            var data = {};
            data[target.attr('id')] = target.attr('value');
            this.model.set(data);
        },

        _cancel: function () {
        },

        _save: function () {
            this.model.save();
        }
    });

    return OrganisationFormLayoutView;
});