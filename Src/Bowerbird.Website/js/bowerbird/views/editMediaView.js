/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// EditMediaView
// -------------

// View that allows user to choose location on a mpa or via coordinates
define(['jquery', 'underscore', 'backbone', 'app', 'models/mediaresource', 'views/mediaresourceitemview', 'loadimage', 'fileupload'], function ($, _, Backbone, app, MediaResource, MediaResourceItemView, loadImage) {

    var EditMediaView = Backbone.View.extend({

        id: 'media-resources-fieldset',

        initialize: function (options) {
            _.extend(this, Backbone.Events);
            _.bindAll(this,
                'render',
                '_initMediaUploader',
                '_onUploadDone',
                '_onSubmitUpload',
                '_onUploadAdd');
            this.mediaResourceItemViews = [];
            this.currentUploadKey = 0;
        },

        render: function () {
            this._initMediaUploader();
            return this;
        },

        _initMediaUploader: function () {
            $('#fileupload').fileupload({
                dataType: 'json',
                paramName: 'file',
                url: '/mediaresources/observationupload',
                add: this._onUploadAdd,
                submit: this._onSubmitUpload,
                done: this._onUploadDone
            });
        },

        _onUploadAdd: function (e, data) {
            var self = this;
            var files = _.filter(data.files, function (file) { return file != null; });
            _.each(files, function (file, index) {
                self.currentUploadKey++;
                var mediaResource = new MediaResource({ Key: self.currentUploadKey.toString() });
                self.model.addMediaResource(mediaResource);
                var mediaResourceItemView = new MediaResourceItemView({ model: mediaResource });
                mediaResourceItemView.on('mediaresourceview:remove', self._onMediaResourceViewRemove);
                self.mediaResourceItemViews.push(mediaResourceItemView);
                $('#media-resource-add-pane').before(mediaResourceItemView.render().el);

                var tempImage = loadImage(
                    data.files[0],
                    function (img) {
                        if (img.type === "error") {
                            //log('Error loading image', img);
                        } else {
                            mediaResourceItemView.showTempMedia(img);
                            self._showMediaResourceItemView(self, mediaResourceItemView, $(img).width(), files.length === index + 1);
                        }
                        //                            if (img instanceof HTMLImageElement) { // FF seems to fire this handler twice, on second time returning error, which we ignore :(
                        //                                mediaResourceItemView.showTempMedia(img);
                        //                            }
                    },
                    { maxHeight: 220 }
                );

                if (!tempImage) {
                    log('No support for file/blob');
                }
            });

            data.submit();
        },

        _showMediaResourceItemView: function (self, mediaResourceItemView, imageWidth, beginAnimation) {
            $('#media-resource-items')
                .queue('mediaQueue', function (next) {
                    self.$el.find('#media-resource-add-pane')
                        .animate(
                        { left: (imageWidth + 15).toString() + 'px' },
                        {
                            duration: 800,
                            queue: false,
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
                $('#media-resource-items').dequeue('mediaQueue');
            }
        },

        _onMediaResourceViewRemove: function (model, view) {
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

        _onSubmitUpload: function (e, data) {
            data.formData = { Key: this.currentUploadKey, OriginalFileName: data.files[0].name };
        },

        _onUploadDone: function (e, data) {
            var mediaResource = this.model.mediaResources.find(function (item) {
                return item.get('Key') === data.result.Key;
            });
            //mediaResource.set(data.result);
            //$('#media-resource-items').animate({ scrollLeft: 100000 });
        }
    });

    return EditMediaView;

});