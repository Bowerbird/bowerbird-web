/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// EditMediaView
// -------------

// View that allows user to choose location on a mpa or via coordinates
define(['jquery', 'underscore', 'backbone', 'app', 'models/mediaresource', 'views/mediaresourceitemview', 'views/videoform', 'loadimage', 'fileupload', 'iframetransport'],
function ($, _, Backbone, app, MediaResource, MediaResourceItemView, VideoForm, loadImage) {
    var EditMediaView = Backbone.View.extend({

        id: 'media-resources-fieldset',

        events: {
            'click #youtube-upload-button': '_showEmbeddedVideo'
        },

        initialize: function (options) {
            log('editMediaView:initialize');
            _.extend(this, Backbone.Events);
            _.bindAll(this,
                'render',
                '_initMediaUploader',
                '_onImageUploadDone',
                '_onSubmitImageUpload',
                '_onImageUploadAdd');
            this.mediaResourceItemViews = [];
            this.currentUploadKey = '';

            app.vent.on('videomediaresourceuploaded:', this._onVideoUploadDone, this);
            app.vent.on('imagemediaresourceuploaded:', this._onImageUploadDone, this);
        },

        render: function () {
            log('ediMediaView:render');
            this._initMediaUploader();
            return this;
        },

        _initMediaUploader: function () {
            log('ediMediaView:_initMediaUploader');
            this.$el.find('#file').fileupload({
                dataType: 'json',
                paramName: 'file',
                url: '/mediaresources',
                add: this._onImageUploadAdd,
                submit: this._onSubmitImageUpload
            });
        },

        filesAdded: 0,  // Used to determine when to fire file upload animations

        _onImageUploadAdd: function (e, data) {
            this.$el.find('.upload-progress').show();

            log('editMediaView:_onImageUploadAdd');
            this.currentUploadKey = app.generateGuid();
            var mediaResource = new MediaResource({ Key: this.currentUploadKey });
            this.model.addMediaResource(mediaResource);

            var mediaResourceItemView = new MediaResourceItemView({ model: mediaResource });
            mediaResourceItemView.on('mediaresourceview:remove', this._onMediaResourceViewRemove);
            this.mediaResourceItemViews.push(mediaResourceItemView);

            var self = this;
            var tempImage = null;

            if (!window.isIEFail) {
                tempImage = loadImage(
                    data.files[0],
                    function (img) {
                        if (img.type === "error") {
                            //log('Error loading image', img);
                        } else {
                            self.filesAdded++;
                            mediaResourceItemView.showTempImageMedia(img);
                            self._showMediaResourceItemView(self, mediaResourceItemView, $(img).width(), self.filesAdded === data.originalFiles.length);
                        }
                    },
                    { maxHeight: 220 }
                );
            }

            if (!tempImage) {
                $(mediaResourceItemView.el).width(280);
                this.filesAdded++;
                this._showMediaResourceItemView(this, mediaResourceItemView, 280, this.filesAdded === data.originalFiles.length);
            }

            data.submit();
        },

        _showEmbeddedVideo: function (e) {
            e.preventDefault();
            $('body').append('<div id="modal-dialog" class="make-invisible"></div>');
            var videoForm = new VideoForm({ el: $('#modal-dialog'), model: new MediaResource() });
            videoForm.on('videouploaded', this._videoUploadAdd, this);
            videoForm.render();
        },

        // once we have a previewable video from the host..
        _videoUploadAdd: function (data, preview) { // data in this case is a MediaResource
            this.$el.find('.upload-progress').show();
            log('editMediaView:_onVideoUploadAdd');
            this.currentUploadKey = app.generateGuid();
            data.set('Key', this.currentUploadKey);
            this.model.addMediaResource(data);


            var mediaResourceItemView = new MediaResourceItemView({ model: data });
            mediaResourceItemView.on('mediaresourceview:remove', this._onMediaResourceViewRemove);
            this.mediaResourceItemViews.push(mediaResourceItemView);
            this.$el.find('.media-resource-items').append(mediaResourceItemView.render().el);

            this.filesAdded++;
            mediaResourceItemView.showVideoMedia(preview);
            this._showMediaResourceItemView(this, mediaResourceItemView, 280, true);

            data.save();
        },

        _showMediaResourceItemView: function (self, mediaResourceItemView, imageWidth, beginAnimation) {
            log('ediMediaView:_showMediaResourceItemView');
            self.$el.find('#media-resource-items')
                .queue('mediaQueue', function (next) {

                    var itemCount = self.$el.find('#media-resource-items > div').length;
                    self.$el.find('#media-resource-items').append(mediaResourceItemView.render().el);

                    if (self.$el.find('#media-resource-items').innerWidth() + self.$el.find('#media-resource-items').scrollLeft() === self.$el.find('#media-resource-items').get(0).scrollWidth) {
                        // Don't do any animation
                        next();
                    }
                    else {
                        var x = self.$el.find('#media-resource-items').get(0).scrollWidth - (self.$el.find('#media-resource-items').innerWidth() + self.$el.find('#media-resource-items').scrollLeft());
                        // Make space for the new item
                        $('#media-resource-items').animate({ scrollLeft: '+=' + x.toString() }, 500, 'swing', next);
                    }
                })
                .queue('mediaQueue', function (next) {
                    $(mediaResourceItemView.el)
                        .animate(
                        { top: '+=250px' },
                        {
                            duration: 800,
                            easing: 'swing',
                            queue: false,
                            complete: next
                        });
                })
                .queue('mediaQueue', function (next) {
                    $(mediaResourceItemView.el).css({ position: 'relative', top: '' });
                    $('#media-resource-items').animate({ scrollLeft: 10000 }, 1);
                    next();
                });

            if (beginAnimation) {
                log('beginnning animation');
                self.filesAdded = 0;
                self.$el.find('#media-resource-items').dequeue('mediaQueue');

                this.$el.find('.upload-progress').hide();
            }
        },

        _onMediaResourceViewRemove: function (model, view) {
            log('ediMediaView:_onMediaResourceViewRemove');
            //            var addToRemoveList = false;
            //            if (app.get('newObservation').mediaResources.find(function (mr) { return mr.id == this.model.id; }) != null) {
            //                addToRemoveList = true;
            //            }
            //            app.get('newObservation').addMediaResources.remove(this.model.id);
            //            app.get('newObservation').mediaResources.remove(this.model.id);
            //            if (addToRemoveList) {
            //                app.get('newObservation').removeMediaResources.add(this.model);
            //            }
            this.model.removeMediaResource(model.id);
            view.remove();
        },

        _onSubmitImageUpload: function (e, data) {
            log('ediMediaView:_onSubmitImageUpload');
            data.formData = { Key: this.currentUploadKey, OriginalFileName: data.files[0].name, MediaType: 'image', Usage: 'observation' };
            if (window.isIEFail) {
                data.formData.ie = true;
            }
        },

        _onImageUploadDone: function (data) {
            log('ediMediaView:_onImageUploadDone', this.model);
            var mediaResource = this.model.mediaResources.find(function (item) {
                return item.get('Key') == data.Key;
            });
            mediaResource.set(data);
        },

        // when we get an uploaded video back from the server, update the id of the mediaresource
        _onVideoUploadDone: function (data) {
            log('ediMediaView:_onVideoUploadDone', data);
            var mediaResource = this.model.mediaResources.find(function (item) {
                return item.get('Key') == data.Key;
            });
            log('mediaResource found: ', mediaResource);
            mediaResource.set(data);
            log('Video Upload set model to: ', data);
        }
    });

    return EditMediaView;

});