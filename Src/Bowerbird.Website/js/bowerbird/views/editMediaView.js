/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// EditMediaView
// -------------

// View that allows user to choose location on a mpa or via coordinates
define(['jquery', 'underscore', 'backbone', 'app', 'models/mediaresource', 'views/mediaresourceitemview', 'views/videoformview', 'loadimage', 'fileupload', 'iframetransport'],
function ($, _, Backbone, app, MediaResource, MediaResourceItemView, VideoFormView, loadImage) {

    var EditMediaView = Backbone.View.extend({

        id: 'media-resources-fieldset',

        events: {
            'click #youtube-upload-button': '_showYouTubeVideoForm',
            'click #vimeo-upload-button': '_showVimeoVideoForm'
        },

        initialize: function (options) {
            _.extend(this, Backbone.Events);
            _.bindAll(this, 'render', '_initMediaUploader', '_onMediaResourceUploadSuccess', '_onMediaResourceUploadFailure', '_onImageUploadAdd', '_showMediaResourceItemView');
            this.mediaResourceItemViews = [];
            app.vent.on('mediaresourceuploadsuccess', this._onMediaResourceUploadSuccess, this);
            app.vent.on('mediaresourceuploadfailure', this._onMediaResourceUploadFailure, this);
        },

        progressCount: 0,

        errorCount: 0,

        render: function () {
            this._initMediaUploader();
            return this;
        },

        _initMediaUploader: function () {
            this.$el.find('#file').fileupload({
                dataType: 'json',
                paramName: 'file',
                url: '/mediaresources',
                add: this._onImageUploadAdd,
                submit: this._onSubmitImageUpload
            });
        },

        _showYouTubeVideoForm: function (e) {
            e.preventDefault();
            this._showVideoForm('youtube');
        },

        _showVimeoVideoForm: function (e) {
            e.preventDefault();
            this._showVideoForm('vimeo');
        },

        _showVideoForm: function (videoProviderName) {
            $('body').append('<div id="modal-dialog"></div>');
            var videoFormView = new VideoFormView({ el: $('#modal-dialog'), videoProviderName: videoProviderName });
            videoFormView.on('videouploaded', this._onVideoUploadAdd, this);
            videoFormView.render();
        },

        _onImageUploadAdd: function (e, data) {
            this.$el.find('.upload-progress').show();
            this._updateProgressCount(1);
            var key = app.generateGuid();
            data.formData = { Key: key, OriginalFileName: data.files[0].name, MediaType: 'image', Usage: 'observation' };
            if (window.isIEFail) {
                data.formData.ie = true;
            }
            var mediaResource = new MediaResource({ Key: key });
            this.model.addMediaResource(mediaResource);

            //            var self = this;
            //            var tempImage = null;

            //            if (!window.isIEFail) {
            //                tempImage = loadImage(
            //                    data.files[0],
            //                    function (img) {
            //                        if (img.type === "error") {
            //                            //log('Error loading image', img);
            //                        } else {
            //                            self.filesAdded++;
            //                            mediaResourceItemView.showTempImageMedia(img);
            //                            self._showMediaResourceItemView(self, mediaResourceItemView, $(img).width(), self.filesAdded === data.originalFiles.length);
            //                        }
            //                    },
            //                    { maxHeight: 220 }
            //                );
            //            }

            //            if (!tempImage) {
            //                $(mediaResourceItemView.el).width(280);
            //                this.filesAdded++;
            //                this._showMediaResourceItemView(this, mediaResourceItemView, 280, this.filesAdded === data.originalFiles.length);
            //            }

            data.submit();
        },

        _onVideoUploadAdd: function (videoId, videoProviderName) {
            this._updateProgressCount(1);
            var mediaResource = new MediaResource({ Key: app.generateGuid(), VideoId: videoId, VideoProviderName: videoProviderName, MediaType: 'video', Usage: 'observation' });
            this.model.addMediaResource(mediaResource);
            mediaResource.save();
        },

        _updateProgressCount: function (value) {
            this.progressCount += value;
            if (this.progressCount > 0) {
                this.$el.find('.upload-status .progress > div').text('Processing ' + this.progressCount + ' file' + (this.progressCount > 1 ? 's' : ''));
                this.$el.find('.upload-status .progress').show();
            }
            else {
                this.$el.find('.upload-status .progress').hide();
            }
        },

        _updateUploadFailure: function (reason) {
            this._updateProgressCount(-1);
            this.errorCount++;
            this.$el.find('.upload-status .message').text(this.errorCount + ' file' + (this.errorCount > 1 ? 's' : '') + ' failed').show();
        },

        _showMediaResourceItemView: function (mediaResource) {
            var mediaResourceItemView = new MediaResourceItemView({ model: mediaResource });
            mediaResourceItemView.on('mediaresourceview:remove', this._onMediaResourceViewRemove);
            this.mediaResourceItemViews.push(mediaResourceItemView);

            var that = this;
            this.$el.find('#media-resource-items')
                .queue(function (next) {
                    var $mediaResourceItems = that.$el.find('#media-resource-items');

                    // Add the new view
                    $mediaResourceItems.append(mediaResourceItemView.render().el);

                    if ($mediaResourceItems.innerWidth() + $mediaResourceItems.scrollLeft() === $mediaResourceItems.get(0).scrollWidth) {
                        // Don't do any animation
                        next();
                    }
                    else {
                        var scrollAmount = ($mediaResourceItems.get(0).scrollWidth - ($mediaResourceItems.innerWidth() + $mediaResourceItems.scrollLeft())) + $mediaResourceItems.scrollLeft();
                        // Make space for the new item
                        $mediaResourceItems.animate(
                            { scrollLeft: scrollAmount },
                            {
                                duration: 100,
                                //easing: 'swing',
                                queue: false,
                                complete: next
                            });

                    }
                })
                .queue(function (next) {
                    // Slide the view down from the top of the div
                    $(mediaResourceItemView.el)
                        .animate(
                        { top: '+=250' },
                        {
                            duration: 800,
                            //easing: 'swing',
                            complete: next
                        });
                })
                .queue(function (next) {
                    // Remove absolute positioning
                    $(mediaResourceItemView.el).css({ position: 'relative', top: '' });
                    that._updateProgressCount(-1);
                    next();
                });
        },

        _onMediaResourceViewRemove: function (model, view) {
            this.model.removeMediaResource(model.id);
            view.remove();
        },

        _onMediaResourceUploadSuccess: function (data) {
            log('editMediaView:_onMediaResourceUploadSuccess', data);
            var mediaResource = this.model.mediaResources.find(function (item) {
                return item.get('Key') === data.Key;
            });
            mediaResource.set(data);
            this._showMediaResourceItemView(mediaResource);
        },

        _onMediaResourceUploadFailure: function (key, reason) {
            log('editMediaView:_onMediaResourceUploadFailure', key, reason);

            var mediaResource = this.model.mediaResources.find(function (item) {
                return item.get('Key') === key;
            });
            this.model.mediaResources.remove(mediaResource);
            this._updateUploadFailure(reason);
        }
    });

    return EditMediaView;

});