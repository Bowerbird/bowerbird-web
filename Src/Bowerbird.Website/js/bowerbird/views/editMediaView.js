/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// EditMediaView
// -------------

// View that allows user to choose location on a mpa or via coordinates
define(['jquery', 'underscore', 'backbone', 'app', 'models/mediaresource', 'views/mediaresourceitemview', 'views/embeddedVideoView', 'loadimage', 'fileupload'],
function ($, _, Backbone, app, MediaResource, MediaResourceItemView, EmbeddedVideoView, loadImage) {
    var EditMediaView = Backbone.View.extend({

        id: 'media-resources-fieldset',

        events: {
            'click #media-resource-embed-button': '_showEmbeddedVideo'
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
            app.vent.on('mediaResourceUploaded:', this._onVideoUploadDone, this);
        },

        render: function () {
            log('ediMediaView:render');
            this._initMediaUploader();
            return this;
        },

        _initMediaUploader: function () {
            log('ediMediaView:_initMediaUploader');
            $('#fileupload').fileupload({
                dataType: 'json',
                paramName: 'file',
                url: '/mediaresources/observationupload',
                add: this._onImageUploadAdd,
                submit: this._onSubmitImageUpload,
                done: this._onImageUploadDone
            });
        },

        filesAdded: 0,  // Used to determine when to fire file upload animations

        _onImageUploadAdd: function (e, data) {
            log('editMediaView:_onImageUploadAdd');
            this.currentUploadKey = app.generateGuid();
            var mediaResource = new MediaResource({ Key: this.currentUploadKey });
            this.model.addMediaResource(mediaResource);

            var mediaResourceItemView = new MediaResourceItemView({ model: mediaResource });
            mediaResourceItemView.on('mediaresourceview:remove', this._onMediaResourceViewRemove);
            this.mediaResourceItemViews.push(mediaResourceItemView);
            this.$el.find('#media-resource-add-pane').before(mediaResourceItemView.render().el);

            var self = this;
            var tempImage = loadImage(
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

            if (!tempImage) {
                $(mediaResourceItemView.el).width(280);
                this.filesAdded++;
                this._showMediaResourceItemView(this, mediaResourceItemView, 280, this.filesAdded === data.originalFiles.length);
            }

            data.submit();
        },

        _showEmbeddedVideo: function () {
            var embeddedVideo = new EmbeddedVideoView({ el: $('#modal-dialog'), model: new MediaResource() });
            embeddedVideo.on('videouploaded', this._videoUploadAdd, this);
            embeddedVideo.render();
        },

        // once we have a previewable video from the host..
        _videoUploadAdd: function (data) { // data in this case is a MediaResource
            log('editMediaView:_onVideoUploadAdd');
            this.currentUploadKey = app.generateGuid();
            data.set('Key', this.currentUploadKey);
            this.model.addMediaResource(data);
            data.save();

            var mediaResourceItemView = new MediaResourceItemView({ model: data });
            mediaResourceItemView.on('mediaresourceview:remove', this._onMediaResourceViewRemove);
            this.mediaResourceItemViews.push(mediaResourceItemView);
            this.$el.find('#media-resource-add-pane').before(mediaResourceItemView.render().el);

            this.filesAdded++;
            mediaResourceItemView.showVideoMedia(data);
            this._showMediaResourceItemView(this, mediaResourceItemView, 280, true);
        },

        _showMediaResourceItemView: function (self, mediaResourceItemView, imageWidth, beginAnimation) {
            log('ediMediaView:_showMediaResourceItemView');
            self.$el.find('#media-resource-items')
                .queue('mediaQueue', function (next) {
                    self.$el.find('#media-resource-add-pane')
                        .animate(
                        { left: (imageWidth + 15).toString() + 'px' },
                        {
                            duration: 800,
                            queue: false,
                            easing: 'swing',
                            step: function () {
                                $('#media-resource-items').animate({ scrollLeft: 10000 }, 1);
                            },
                            complete: next
                        });
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
                    self.$el.find('#media-resource-add-pane').css({ left: '' });
                    $(mediaResourceItemView.el).css({ position: 'relative', top: '' });
                    $('#media-resource-items').animate({ scrollLeft: 10000 }, 1);
                    next();
                });

            if (beginAnimation) {
                log('beginnning animation');
                self.filesAdded = 0;
                self.$el.find('#media-resource-items').dequeue('mediaQueue');
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
            data.formData = { Key: this.currentUploadKey, OriginalFileName: data.files[0].name };
        },

        _onImageUploadDone: function (e, data) {
            log('ediMediaView:_onImageUploadDone', this.model);
            var mediaResource = this.model.mediaResources.find(function (item) {
                return item.get('Key') === data.result.Key;
            });
            mediaResource.set(data.result);
            log('Photo Latitude: ' + data.result.PhotoLatitude);
            log('Photo Longitude: ' + data.result.PhotoLongitude);
            //$('#media-resource-items').animate({ scrollLeft: 100000 });
            //app.vent.trigger('observationmedia:uploaded', mediaResource);
        },

        // when we get an uploaded video back from the server, update the id of the mediaresource
        _onVideoUploadDone: function (data) {
            log('ediMediaView:_onVideoUploadDone', data);
            var mediaResource = this.model.mediaResources.find(function (item) {
                return item.get('Key') === data.Metadata.Key;
            });
            log('mediaResource found: ', mediaResource);
            mediaResource.set(data);
            //this.model.addMediaResource(mediaResource);
        }
    });

    return EditMediaView;

});