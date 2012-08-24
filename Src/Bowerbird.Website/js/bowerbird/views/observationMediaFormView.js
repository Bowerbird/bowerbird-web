/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ObservationMediaFormView
// ------------------------

define(['jquery', 'underscore', 'backbone', 'app', 'models/mediaresource', 'views/observationmediaitemview', 'views/videoformview', 'collections/mediaresourcecollection', 'fileupload', 'iframetransport'],
function ($, _, Backbone, app, MediaResource, ObservationMediaItemView, VideoFormView, MediaResourceCollection) {

    var ObservationMediaFormView = Backbone.Marionette.CompositeView.extend({
        id: 'media-fieldset',

        itemView: ObservationMediaItemView,

        events: {
            'click #youtube-upload-button': '_showYouTubeVideoForm',
            'click #vimeo-upload-button': '_showVimeoVideoForm'
        },

        initialize: function () {
            _.bindAll(this, '_onMediaResourceUploadSuccess', '_onMediaResourceUploadFailure', '_onImageUploadAdd', '_onVideoUploadAdd');

            this.currentUploads = new MediaResourceCollection();
            this.failedUploads = new MediaResourceCollection();

            this.currentUploads.on('add', this._onCurrentUploadAdded, this);
            this.failedUploads.on('add', this._onFailedUploadAdded, this);

            app.vent.on('mediaresourceuploadsuccess', this._onMediaResourceUploadSuccess, this);
            app.vent.on('mediaresourceuploadfailure', this._onMediaResourceUploadFailure, this);
        },

        onRender: function () {
            this.$el.find('#file').fileupload({
                dataType: 'json',
                paramName: 'File',
                url: '/mediaresources',
                add: this._onImageUploadAdd
            });
        },

        appendHtml: function (collectionView, itemView) {
            itemView.on('removemedia', this._onMediaRemove, this);
            itemView.on('newprimarymedia', this._onNewPrimaryMedia, this);

            var that = this;
            this.$el.find('.observation-media-items')
                .queue(function (next) {
                    var $mediaItems = that.$el.find('.observation-media-items');

                    // Add the new view
                    $mediaItems.append(itemView.el);

                    if ($mediaItems.innerWidth() + $mediaItems.scrollLeft() === $mediaItems.get(0).scrollWidth) {
                        // Don't do any animation
                        next();
                    }
                    else {
                        var scrollAmount = ($mediaItems.get(0).scrollWidth - ($mediaItems.innerWidth() + $mediaItems.scrollLeft())) + $mediaItems.scrollLeft() + 500;
                        // Make space for the new item
                        $mediaItems.animate(
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
                    $(itemView.el)
                        .animate(
                        { top: '+=250' },
                        {
                            duration: 800,
                            //easing: 'swing',
                            queue: false,
                            complete: next
                        });
                })
                .queue(function (next) {
                    // Remove absolute positioning
                    $(itemView.el).css({ position: 'relative', top: '' });

                    var mediaResource = that.currentUploads.find(function (item) {
                        return item.get('Key') === itemView.model.mediaResource.get('Key');
                    });
                    that.currentUploads.remove(mediaResource);

                    that._updateProgress();
                    //next();

                    var $mediaItems = that.$el.find('.observation-media-items');
                    var scrollAmount = ($mediaItems.get(0).scrollWidth - ($mediaItems.innerWidth() + $mediaItems.scrollLeft())) + $mediaItems.scrollLeft();

                    itemView.start();

                    // Make space for the new item
                    $mediaItems.animate(
                            { scrollLeft: scrollAmount },
                            {
                                duration: 100,
                                //easing: 'swing',
                                queue: false,
                                complete: next
                            });
                });
        },

        _onMediaRemove: function (mediaItemView) {
            var that = this;
            this.$el.find('.observation-media-items')
                .queue(function (next) {
                    // Slide the view down out of the div
                    $(mediaItemView.el)
                        .animate(
                            { top: '+=250' },
                            {
                                duration: 800,
                                //easing: 'swing',
                                complete: next
                            });
                })
                .queue(function (next) {
                    that.model.removeMedia(mediaItemView.model);
                    next();
                });
        },

        _onNewPrimaryMedia: function (media) {
            this.model.setPrimaryMedia(media);
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
            var key = app.generateGuid();
            this.currentUploads.add({ Key: key });

            data.formData = {
                Key: key,
                Type: 'file', 
                Usage: 'contribution',
                FileName: data.files[0].name
            };
            if (window.isIEFail) {
                data.formData.ie = true;
            }

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
            var key = app.generateGuid();
            this.currentUploads.add({ Key: key });

            $.ajax({
                url: '/mediaresources',
                dataType: 'json',
                type: 'post',
                data: {
                    Key: key,
                    Type: 'externalvideo',
                    Usage: 'contribution',
                    VideoId: videoId,
                    VideoProviderName: videoProviderName
                }
            });
        },

        _onCurrentUploadAdded: function (mediaResource) {
            this._updateProgress();
        },

        _onFailedUploadAdded: function (mediaResource) {
            var failedCount = this.failedUploads.length;
            this.$el.find('.upload-status .message').text(failedCount + ' file' + (failedCount > 1 ? 's' : '') + ' failed').show();
            this._updateProgress();
        },

        _updateProgress: function () {
            if (this.model.media.length > 0) {
                this.$el.find('.observation-media-items-label').hide();
            } else {
                this.$el.find('.observation-media-items-label').show();
            }

            var currentCount = this.currentUploads.length;
            if (this.currentUploads.length > 0) {
                this.$el.find('.upload-status .progress > div').text('Processing ' + currentCount + ' file' + (currentCount > 1 ? 's' : ''));
                this.$el.find('.upload-status .progress').show();
            }
            else {
                this.$el.find('.upload-status .progress').hide();
            }
        },

        _onMediaResourceUploadSuccess: function (data) {
            var mediaResource = this.currentUploads.find(function (item) {
                return item.get('Key') === data.Key;
            });
            mediaResource.set(data);
            this.model.addMedia(mediaResource, '', app.authenticatedUser.defaultLicence);
            this._updateProgress();
        },

        _onMediaResourceUploadFailure: function (key, reason) {
            var mediaResource = this.currentUploads.find(function (item) {
                return item.get('Key') === key;
            });
            this.currentUploads.remove(mediaResource);
            this.failedUploads.add(mediaResource);
            this._updateProgress();
        },

        onClose: function () {
            app.vent.off('mediaresourceuploadsuccess', this._onMediaResourceUploadSuccess, this);
            app.vent.off('mediaresourceuploadfailure', this._onMediaResourceUploadFailure, this);
        }
    });

    return ObservationMediaFormView;

});