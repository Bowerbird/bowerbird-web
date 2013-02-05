/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ObservationMediaFormView
// ------------------------

define(['jquery', 'underscore', 'backbone', 'app', 'models/mediaresource', 'views/observationmediaitemformview', 'views/videoformview', 'fileupload', 'iframetransport', 'progress'],
function ($, _, Backbone, app, MediaResource, ObservationMediaItemFormView, VideoFormView) {

    var MediaUpload = Backbone.Model.extend({
        defaults: {
            progressStatus: 'waiting',
            successStatus: ''
        }
    });

    var MediaUploadCollection = Backbone.Collection.extend({
        model: MediaUpload
    });

    var ObservationMediaFormView = Backbone.Marionette.CompositeView.extend({
        id: 'media-details',

        itemView: ObservationMediaItemFormView,

        events: {
            'click #youtube-upload-button': '_showYouTubeVideoForm',
            'click #vimeo-upload-button': '_showVimeoVideoForm',
            'click #add-media-button': '_showAddMediaOptions',
            'click .file-upload-button': '_clickFileUpload'
        },

        initialize: function () {
            _.bindAll(this, '_onMediaResourceUploadSuccess', '_onMediaResourceUploadFailure', '_onFileUploadAdd', '_onFileUploadSend', '_onFileUploadDone', '_onFileUploadFail', '_onVideoUploadAdd', '_showAddMediaOptions', '_clickFileUpload');

            this.mediaUploads = new MediaUploadCollection();
            this.mediaUploads.on('change:progressStatus', this._updateProgress, this);

            app.vent.on('mediaresourceuploadsuccess', this._onMediaResourceUploadSuccess, this);
            app.vent.on('mediaresourceuploadfailure', this._onMediaResourceUploadFailure, this);

            this.isRendering = true;
        },

        onRender: function () {
            this.$el.find('#file').fileupload({
                type: 'POST',
                dataType: 'json',
                paramName: 'File',
                url: '/mediaresources',
                add: this._onFileUploadAdd, // on file selected
                send: this._onFileUploadSend, // on request submit
                done: this._onFileUploadDone, // on successful upload
                fail: this._onFileUploadFail // on errored upload
            });

            this.$el.find('#add-media-button ul li a, #add-media-button ul li > div').tipsy({ gravity: 'e', fade: true });

            this.isRendering = false;
        },

        appendHtml: function (collectionView, itemView) {
            itemView.on('removemedia', this._onMediaRemove, this);
            itemView.on('newprimarymedia', this._onNewPrimaryMedia, this);

            if (this.isRendering) {
                var elem = this.$el.find('[class*="observation-media-item-' + itemView.model.get('MediaResourceId') + '"]');
                itemView.setElement(elem);
            }
            else {
                var that = this;
                this.$el.find('.observation-media-items')
                    .queue(function (next) {
                        var $mediaItems = that.$el.find('.observation-media-items');

                        // Add the new view
                        $mediaItems.append(itemView.el);

                        if ($mediaItems.innerWidth() + $mediaItems.scrollLeft() === $mediaItems.get(0).scrollWidth) {
                            // Don't do any scrolling, just move to next step
                            next();
                        } else {
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

                        var upload = that.mediaUploads.get(itemView.model.mediaResource.get('Key'));

                        upload.set('progressStatus', 'complete');

                        var $mediaItems = that.$el.find('.observation-media-items');
                        var scrollAmount = ($mediaItems.get(0).scrollWidth - ($mediaItems.innerWidth() + $mediaItems.scrollLeft())) + $mediaItems.scrollLeft();

                        itemView.start();

                        // Scroll the new item into view
                        $mediaItems.animate(
                            { scrollLeft: scrollAmount },
                            {
                                duration: 100,
                                //easing: 'swing',
                                queue: false,
                                complete: next
                            });
                    });
            }
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
                    that._showOrHideMediaLabel();
                    next();
                });
        },

        _onNewPrimaryMedia: function (media) {
            this.model.setPrimaryMedia(media);
        },

        _showYouTubeVideoForm: function (e) {
            e.preventDefault();
            this._showVideoForm('youtube');
            return false;
        },

        _showVimeoVideoForm: function (e) {
            e.preventDefault();
            this._showVideoForm('vimeo');
            return false;
        },

        _showVideoForm: function (videoProviderName) {
            app.vent.trigger('close-sub-menus');
            this.$el.find('#add-media-button ul li a, #add-media-button ul li > div').tipsy.revalidate();
            $('body').append('<div id="modal-dialog"></div>');
            var videoFormView = new VideoFormView({ el: $('#modal-dialog'), videoProviderName: videoProviderName });
            videoFormView.on('videouploaded', this._onVideoUploadAdd, this);
            videoFormView.render();
        },

        _showAddMediaOptions: function (e) {
            e.preventDefault();
            app.vent.trigger('close-sub-menus');
            this.$el.find('.add-media-button').addClass('active');
            return false;
        },

        _clickFileUpload: function (e) {
            app.vent.trigger('close-sub-menus');
            this.$el.find('#add-media-button ul li a, #add-media-button ul li > div').tipsy.revalidate();
            e.stopPropagation();
            return true;
        },

        _onFileUploadAdd: function (e, data) {
            var key = app.generateGuid();

            this.mediaUploads.add({
                id: key,
                mediaType: 'file',
                filename: data.files[0].name
            });

            data.formData = {
                Key: key,
                Type: 'file',
                Usage: 'contribution',
                Filename: data.files[0].name
            };
            if (window.isIEFail) {
                data.formData.ie = true;
            }

            data.submit();
        },

        _onFileUploadSend: function (e, data) {
            var upload = this.mediaUploads.get(data.formData.Key || data.formData[0].value); // Not sure why IE decides to make JSON an array?

            upload.set({
                progressStatus: 'uploading'
            });
        },

        _onFileUploadDone: function (e, data) {
            var upload = this.mediaUploads.get(data.formData.Key || data.formData[0].value);

            if (data.result.Model.Success === true) {
                upload.set({
                    progressStatus: 'processing'
                });
            } else {
                upload.set({
                    progressStatus: 'complete',
                    successStatus: 'fail'
                });
            }
        },

        _onFileUploadFail: function (e, data) {
            var upload = this.mediaUploads.get(data.formData.Key);

            upload.set({
                progressStatus: 'complete',
                successStatus: 'fail'
            });
        },

        _onVideoUploadAdd: function (videoId, videoProviderName) {
            var key = app.generateGuid();

            this.mediaUploads.add({
                id: key,
                mediaType: 'externalvideo',
                videoId: videoId,
                videoProviderName: videoProviderName
            });

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

        _updateProgress: function (mediaUpload) {
            this._showOrHideMediaLabel();

            var uploads = this.mediaUploads.map(function (upload) {
                return {
                    id: upload.id,
                    progressStatus: upload.get('progressStatus'),
                    successStatus: upload.get('successStatus')
                };
            });
            var that = this;

            this.$el.find('#upload-progress-status')
                    .queue(function (next) {
                        var totalCount = uploads.length;
                        var waitingCount = _.filter(uploads, function (upload) { return upload.progressStatus === 'waiting'; }).length;
                        var uploadingCount = _.filter(uploads, function (upload) { return upload.progressStatus === 'uploading'; }).length;
                        var processingCount = _.filter(uploads, function (upload) { return upload.progressStatus === 'processing'; }).length;
                        var loadingCount = _.filter(uploads, function (upload) { return upload.progressStatus === 'loading'; }).length;
                        var completeCount = _.filter(uploads, function (upload) { return upload.progressStatus === 'complete'; }).length;

                        if (completeCount < totalCount) {
                            var total = totalCount * 5;
                            var current = waitingCount * 0.1 + (uploadingCount * 1) + (processingCount * 3) + (loadingCount * 4) + (completeCount * 5);

                            var progress = Math.round((current * 100) / total);

                            that.$el.find('#upload-progress-message').text('Adding ' + totalCount + ' file' + (totalCount > 1 ? 's' : ''));

                            log('progress', progress + '%', uploads, mediaUpload);
                            that.$el.find('#upload-progress-bar .ui-progress').css('width', progress + '%');

                            that.$el.find('#upload-progress-status').css('visibility', 'visible');
                        }
                        else {
                            log('progress', '100%', uploads, mediaUpload);
                            that.$el.find('#upload-progress-bar .ui-progress').css('width', '100%');

                            setTimeout((function () {
                                that.$el.find('#upload-progress-status').css('visibility', 'hidden');
                                that.mediaUploads.reset(null, { silent: true });
                            }), 200);
                        }

                        if (mediaUpload.get('successStatus') === 'fail') {
                            var errors = that.mediaUploads.filter(function(item) {
                                return item.get('successStatus') === 'fail';
                            });
                            
                            errors = _.map(errors, function (item) {
                                return {
                                    Field: 'Media-' + item.id,
                                    Message: 'The file "' + item.get('filename') + '" failed to be added. The supported file types for uploading are JPEG, TIFF, PNG and MP3. If uploading a video, please ensure it is a valid Youtube or Vimeo video.'
                                };
                            });

                            that.trigger('upload-error', mediaUpload, errors);
                        }

                        next();
                    });
        },

        _showOrHideMediaLabel: function () {
            if (this.model.media.length > 0) {
                this.$el.find('.observation-media-items-label').hide();
            } else {
                this.$el.find('.observation-media-items-label').show();
            }
        },

        _onMediaResourceUploadSuccess: function (data) {
            var that = this;

            setTimeout((function () {
                var upload = that.mediaUploads.get(data.Key);

                upload.set({
                    progressStatus: 'loading',
                    successStatus: 'success',
                    mediaResource: new MediaResource(data)
                });

                that.model.addMedia(upload.get('mediaResource'), '', app.authenticatedUser.defaultLicence);
            }), 1000);
        },

        _onMediaResourceUploadFailure: function (key, reason) {
            var upload = this.mediaUploads.get(data.Key);

            upload.set({
                progressStatus: 'complete',
                successStatus: 'fail',
                errorMessage: reason
            });
        },

        onClose: function () {
            app.vent.off('mediaresourceuploadsuccess', this._onMediaResourceUploadSuccess, this);
            app.vent.off('mediaresourceuploadfailure', this._onMediaResourceUploadFailure, this);
        }
    });

    return ObservationMediaFormView;

});