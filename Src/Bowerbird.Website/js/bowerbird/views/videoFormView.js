/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// VideoFormView
// -------------

define(['jquery', 'underscore', 'backbone', 'app', 'ich'],
function ($, _, Backbone, app, ich) {

    var YouTubeVideoProvider = function (options) {
        this.onGetVideoCallback = options.onGetVideoCallback;

        this.getJSON = function () {
            return {
                name: 'YouTube',
                placeholderUri: 'http://www.youtube.com'
            };
        };

        this.getVideoId = function (value) {
            /*
            Handles the following URLs:
            http://www.youtube.com/watch?v=0zM3nApSvMg&feature=feedrec_grec_index
            http://www.youtube.com/user/IngridMichaelsonVEVO#p/a/u/1/QdK8U-VIH_o
            http://www.youtube.com/v/0zM3nApSvMg?fs=1&amp;hl=en_US&amp;rel=0
            http://www.youtube.com/watch?v=0zM3nApSvMg#t=0m10s
            http://www.youtube.com/embed/0zM3nApSvMg?rel=0
            http://www.youtube.com/watch?v=0zM3nApSvMg
            http://youtu.be/0zM3nApSvMg
            */
            var uriRegExp = /^.*(youtu.be\/|v\/|u\/\w\/|embed\/|watch\?v=)([^#\&\?]*).*/;
            var uriMatch = value.match(uriRegExp);
            if (uriMatch && uriMatch[2].length == 11) {
                return uriMatch[2];
            }
            return '';
        };

        this.getVideo = function (videoId) {
            var that = this;
            $.ajax({
                type: "GET",
                dataType: "jsonp",
                url: 'http://gdata.youtube.com/feeds/api/videos/' + videoId + '?v=2&alt=json-in-script',
                success: function (data) {
                    log('youtube video data', data);
                    var videoId = data.entry['media$group']['yt$videoid']['$t'];
                    that.onGetVideoCallback(videoId);
                }
            });
        };
    };

    var VimeoVideoProvider = function (options) {
        this.onGetVideoCallback = options.onGetVideoCallback;

        this.getJSON = function () {
            return {
                name: 'Vimeo',
                placeholderUri: 'http://www.vimeo.com'
            };
        };

        this.getVideoId = function (value) {
            /*
            Handles the following URLs:
            http://www.vimeo.com/7058755
            */
            var uriRegExp = /http:\/\/(www\.)?vimeo.com\/(\d+)($|\/)/;
            var uriMatch = value.match(uriRegExp);
            if (uriMatch) {
                return uriMatch[2];
            }
            return '';
        };

        this.getVideo = function (videoId) {
            var that = this;
            $.ajax('http://vimeo.com/api/v2/video/' + videoId + '.json', { type: 'GET', dataType: 'jsonp' })
                .done(function (data) {
                    log('vimeo video data', data);
                    that.onGetVideoCallback(data[0].id);
                })
                .fail(function (x) { log(x); });
        };
    };

    var VideoFormView = Backbone.Marionette.ItemView.extend({
        id: 'video-form',

        template: 'VideoForm',

        events: {
            'click .cancel-button': '_cancel',
            'click .close': '_cancel',
            'click .add-button': '_add'
        },

        provider: null,

        isLoading: false,

        serializeData: function () {
            return {
                Model: this.provider.getJSON()
            };
        },

        initialize: function (options) {
            _.bindAll(this, '_loadVideo', '_onGetYouTubeVideo', '_onGetVimeoVideo', '_updateVideoStatus');

            if (options.provider === 'youtube') {
                this.provider = new YouTubeVideoProvider({ onGetVideoCallback: this._onGetYouTubeVideo });
            } else if (options.provider === 'vimeo') {
                this.provider = new VimeoVideoProvider({ onGetVideoCallback: this._onGetVimeoVideo });
            }
        },

        onRender: function () {
            var that = this;
            this.$el.find('#VideoUri').on('change keyup', function (e) {
                log('change keyup', e);
                that._loadVideo($(this).val());
            });
            this.$el.find('#VideoUri').on('paste', function (e) {
                log('paste', e);
                setTimeout(function () {
                    that._loadVideo(that.$el.find('#VideoUri').val());
                }, 100);
            });
            return this;
        },

        _loadVideo: function (value) {
            if (value === '') {
                this.model.set('VideoUri', '');
                this._updateVideoStatus('none');
                return;
            }

            var videoId = this.provider.getVideoId(value);

            if (videoId === '') {
                this.model.set('VideoUri', '');
                this._updateVideoStatus('error');
            }

            if (this.model.get('VideoUri') !== videoId) {
                this.model.set('VideoUri', videoId);
                this._updateVideoStatus('loading');
                this.provider.getVideo(videoId);
            }
        },

        _updateVideoStatus: function (status) {
            var html = '';
            switch (status) {
                case 'none':
                    html = '<p>Please enter a ' + this.provider.getJSON().name + ' video link</p>';
                    break;
                case 'loading':
                    html = '<p><img src="/img/loader.png" alt="" />Loading ' + this.provider.getJSON().name + ' video...</p>';
                    break;
                case 'error':
                    html = '<p>The ' + this.provider.getJSON().name + ' video link you entered doesn\'t seem to be working. Please correct the link and try again.</p>';
                    break;
                default:
                    return;
            }
            this.$el.find('.video-preview').empty().html(html);
        },

        _onGetVimeoVideo: function (videoId) {
            this.$el.find('.video-preview').html('<iframe id="vimeo-video-player" type="text/html" width="550" height="310" src="http://player.vimeo.com/video/' + videoId + '" frameborder="0" allowfullscreen>');
        },

        _onGetYouTubeVideo: function (videoId) {
            this.$el.find('.video-preview').html('<iframe id="youtube-video-player" type="text/html" width="550" height="310" src="http://www.youtube.com/embed/' + videoId + '?controls=0&modestbranding=1&rel=0&showinfo=0" frameborder="0" allowfullscreen>');
        },

        _cancel: function () {
            this.remove();
        },

        _add: function () {
            if (this.model.get('VideoUri') !== '') {
                this.trigger('videouploaded', this.model);
                this.remove();
            }
        }
    });

    return VideoFormView;
});