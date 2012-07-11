/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// EmbeddedVideoView
// -----------------

// Shows an individual project item
define(['jquery', 'underscore', 'backbone', 'app', 'ich'],
function ($, _, Backbone, app, ich) {
    var EmbeddedVideoView = Backbone.Marionette.ItemView.extend({

        id: 'embedded-video',

        template: 'EmbeddedVideo',

        events: {
            'click button#embed-video-clear-button': '_clear',
            'click button#embed-video-cancel-button': '_cancel',
            'click button#embed-video-next-button': '_next',
            'change input#embed-video-link-input': '_updateLink',
            'change textarea#embed-video-description-input': '_updateDescription'
        },

        initialize: function (options) {
            log('embeddedVideoView:initialize', options);
            this.ValidVideo = false;
            this.VisiblePreview = false;
            this.Preview = '';
            this.model.set('MediaType', 'video');
            //this.model.on('change:Metadata', this.onMediaResourceFilesChanged, this);
            this.model.on('change:Files', this.onMediaResourceFilesChanged, this);
        },

        onRender: function () {
            log('embeddedVideoView:onRender');
            //this._resetView();
            this._showElement($('#modal-dialog'));
            this._hideElement($('div#embed-video-preview'));
            return this;
        },

        // fire this event when the link has been updated
        _updateLink: function () {
            log('embeddedVideoView:_updateLink');
            this._previewVideo($('input#embed-video-link-input').val());
        },

        _previewVideo: function (linkUri) {
            log('EmbeddedVideo.previewVideo', linkUri);
            var that = this;
            $.when(this._getVideoPreview(linkUri))
            .done(function (data) {
                log(data, data.PreviewTags, data.success);
                that.Preview = data.PreviewTags;
                that.ValidVideo = data.success;
            });

            this.model.set('LinkUri', linkUri);
        },

        _getVideoPreview: function (url) {
            log('EmbeddedVideo.getVideoPreview');
            var deferred = new $.Deferred();
            var params = {};
            params['url'] = url;
            $.ajax({
                url: '/videopreview',
                type: "POST",
                data: params
            }).done(function (data) {
                deferred.resolve(data);
            });
            return deferred.promise();
        },

        // Fire this event when the description has been updated
        _updateDescription: function () {
            this.model.set('Description', $('input#embed-video-description-input').val());
        },

        // remove the current video preview and reset the form..
        _clear: function () {
            this._resetView();
        },

        // no longer adding a new video
        _cancel: function () {
            this._hideElement($('div#modal-dialog'));
            this._cleanup();
        },

        // set form and model back to their original state
        _resetView: function () {
            log('embeddedVideoView:_resetView');
            this.VisiblePreview = false;
            $('input#embed-video-link-input').val('');
            $('#embed-video-description-input').val('');
            $('div#embed-video-player').html('');
            this._hideElement($('div#embed-video-preview'));
        },

        // this button itself is modal, in that click it once to show a preview, 
        // then if preview is visible, click again to save the viewed video.
        _next: function () {
            log('embeddedVideoView:_next');
            // if we've got a valid video, and we've seen it, we are clicking to save
            if (this.ValidVideo && this.VisiblePreview) {
                this._save();
            }
            // if we've loaded a new video, but haven't seen it, display it:
            else if (this.ValidVideo) {
                var src = this.Preview;
                this._showElement($('div#embed-video-preview'));
                $('div#embed-video-player').empty();
                this.$el.find('div#embed-video-player').append(ich.VideoPreview({ Width: 520, Height: 400, Source: src }));
                // now we've seen it, so next click we want to save it.
                this.VisiblePreview = true;
            }
        },

        // trigger the editMediaView to save its model with this model in it..
        _save: function () {
            this.model.set('Description', $('#embed-video-description-input').val());
            this.trigger('videouploaded', this.model, this.Preview);
            //this.model.save();
            this._cleanup();
        },

        _cleanup: function () {
            this.remove();
        },

        _hideElement: function (el) {
            $(el).addClass('make-invisible');
            $(el).removeClass('make-visible');
        },

        _showElement: function (el) {
            $(el).addClass('make-visible');
            $(el).removeClass('make-invisible');
        }

    });

    return EmbeddedVideoView;
});