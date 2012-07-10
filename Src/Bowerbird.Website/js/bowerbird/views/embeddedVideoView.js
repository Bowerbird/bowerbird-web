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
            this.model.on('change:Metadata', this.onMediaResourceFilesChanged, this);
            this.model.set('MediaType', 'video');
        },

        onRender: function () {
            log('embeddedVideoView:onRender');
            this._resetView();
            this._showElement($('#modal-dialog'));
            return this;
        },

        // fire this event when the link has been updated
        _updateLink: function () {
            log('embeddedVideoView:_updateLink');
            this.model.previewVideo($('input#embed-video-link-input').val());
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
            this._resetView();
            this._hideElement($('div#modal-dialog'));
            this._cleanup();
        },

        // set form and model back to their original state
        _resetView: function () {
            log('embeddedVideoView:_resetView');
            $('input#embed-video-link-input').val('');
            $('#embed-video-description-input').val('');
            $('div#embed-video-player').html('');
            this.model.set('VisiblePreview', false);
            this._hideElement($('div#embed-video-preview'));
        },

        _next: function () {
            log('embeddedVideoView:_next');
            // if we've got a valid video, and we've seen it:
            if (this.model.get('ValidVideo') && this.model.get('VisiblePreview')) {
                this._save();
            }
            // if we've loaded a new video, display it:
            else if (this.model.get('ValidVideo')) {
                var src = this.model.get('Preview');
                this._showElement($('div#embed-video-preview'));
                $('div#embed-video-player').empty();
                this.$el.find('div#embed-video-player').append(ich.VideoPreview({ Width: 520, Height: 400, Source: src }));
                this.model.set('VisiblePreview', true);
            }
        },

        // trigger the editMediaView to save its model with this model in it..
        _save: function () {
            this.model.set('Description', $('#embed-video-description-input').val());
            this.trigger('videouploaded', this.model);
            this._cleanup();
        },

        _cleanup: function () {
            this.remove();
            //this.unbind();
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