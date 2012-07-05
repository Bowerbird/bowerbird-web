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
            'click #embed-video-clear-button': '_clear', // first form part buttons
            'click #embed-video-cancel-button': '_cancel',
            'click #embed-video-view-button': '_viewVideo',
            'change input#embed-video-link-input': '_updateLinkToModel',
            'click #embed-video-save-button': '_saveEmbeddedVideo', // second form part buttons
            'click #embed-video-description-clear-button': '_descriptionClear',
            'click #embed-video-edit-button': '_previewCancel'
        },

        initialize: function (options) {
            log('embeddedVideoView:initialize', options);
            this.model.on('change:Metadata', this.onMediaResourceFilesChanged, this);
        },

        onRender: function () {
            log('embeddedVideoView:onRender');
            this._showElement($('#modal-dialog'));
            this._resetView();
            return this;
        },

        _saveEmbeddedVideo: function () {
            e.preventDefault();
        },

        _updateLinkToModel: function () {
            if (this.model.validateLink($('#embed-video-link-input').val())) {
                var embedScript = this.model.get('EmbedScript')
                $('div#embed-video-player').html(embedScript);
            }
            else {
                $('#embed-video-link-input').val(this.model.get('ErrorMessage'));
            }
        },

        _viewVideo: function () {
            var embeddedText = $(':input:#embed-video-script-input').val();
            if (embeddedText != '') {
                $('div#embed-video-player').html('<center>' + embeddedText + '</center>');
                this._hideElement($('#embed-video-link'));
                this._showElement($('#embed-video-preview'));
            }
        },

        _cancel: function () {
            //$(':input:#embed-video-script-input').val('');
            $(':input:#embed-video-link-input').val('');
            $('div#embed-video-player').html('');
            this._resetView();
            this._hideElement($('#modal-dialog'));
        },

        _previewCancel: function () {
            this._showElement($('#embed-video-link'));
            this._hideElement($('#embed-video-preview'));
        },

        _clear: function () {
            //$(':input:#embed-video-script-input').val('');
            $(':input:#embed-video-link-input').val('');
            $('div#embed-video-player').html('');
            this._resetView();
        },

        _descriptionClear: function () {
            $(':input:#embed-video-title-input').val('');
            $(':input:#embed-video-description-input').val('');
        },

        _resetView: function () {
            this._showElement($('#embed-video-link'));
            this._hideElement($('#embed-video-preview'));
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