/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// VideoFormView
// -------------

define(['jquery', 'underscore', 'backbone', 'app', 'ich', 'jsonp'],
function ($, _, Backbone, app, ich) {

    var YouTubeVideoProvider = function (options) {
        this.options = options;

        this.getJSON = function () {
            return {
                name: 'YouTube',
                placeholderUri: 'http://www.youtube.com',
                exampleUri: 'http://www.youtube.com/watch?v=OQSNhk5ICTI'
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
            $.jsonp({
                url: 'http://gdata.youtube.com/feeds/api/videos/' + videoId + '?v=2&alt=json-in-script&callback=?',
                context: this,
                success: function (data, status) {
                    log('youtube video data', data, status);
                    this.options.onGetVideoSuccess(data.entry['media$group']['yt$videoid']['$t']);
                },
                error: function (data, status) {
                    log('failed to load youtube video', data, status);
                    this.options.onGetVideoError(data);
                }
            });
        };
    };

    var VimeoVideoProvider = function (options) {
        this.options = options;

        this.getJSON = function () {
            return {
                name: 'Vimeo',
                placeholderUri: 'http://www.vimeo.com',
                exampleUri: 'http://www.vimeo.com/40000072'
            };
        };

        this.getVideoId = function (value) {
            /*
            Handles the following URLs:
            http://www.vimeo.com/7058755
            https://www.vimeo.com/7058755
            http://vimeo.com/7058755
            */
            var uriRegExp = /^.*(www\.)?vimeo.com\/(\d+)($|\/)/;
            var uriMatch = value.match(uriRegExp);
            if (uriMatch) {
                return uriMatch[2];
            }
            return '';
        };

        this.getVideo = function (videoId) {
            $.jsonp({
                url: 'http://vimeo.com/api/v2/video/' + videoId + '.json?callback=?',
                context: this,
                success: function (data, status) {
                    log('vimeo video data', data, status);
                    this.options.onGetVideoSuccess(data[0].id);
                },
                error: function (data, status) {
                    log('failed to load vimeo video', data, status);
                    this.options.onGetVideoError(data);
                }
            });
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

        videoId: '',

        serializeData: function () {
            return {
                Model: this.provider.getJSON()
            };
        },

        initialize: function (options) {
            _.bindAll(this, '_loadVideo', '_onGetYouTubeVideo', '_onGetVimeoVideo', '_onGetVideoError', '_updateVideoStatus');

            if (options.videoProviderName === 'youtube') {
                this.provider = new YouTubeVideoProvider({ onGetVideoSuccess: this._onGetYouTubeVideo, onGetVideoError: this._onGetVideoError });
            } else if (options.videoProviderName === 'vimeo') {
                this.provider = new VimeoVideoProvider({ onGetVideoSuccess: this._onGetVimeoVideo, onGetVideoError: this._onGetVideoError });
            }
        },

        onRender: function () {
            var that = this;
            this.$el.find('#VideoUri').on('change keyup', function (e) {
                that._loadVideo($(this).val());
            });
            this.$el.find('#VideoUri').on('paste', function (e) {
                setTimeout(function () {
                    that._loadVideo(that.$el.find('#VideoUri').val());
                }, 100);
            });

            this.$el.find('.add-button').attr('disabled', 'disabled');

            return this;
        },

        _loadVideo: function (value) {
            if (value === '') {
                this.videoId = '';
                this._updateVideoStatus('none');
                return;
            }

            var newVideoId = this.provider.getVideoId(value);

            if (newVideoId === '') {
                this.videoId = '';
                this._updateVideoStatus('error');
                return;
            }

            if (this.videoId !== newVideoId) {
                this.videoId = newVideoId;
                this._updateVideoStatus('loading');
                this.provider.getVideo(this.videoId);
            }
        },

        _updateVideoStatus: function (status, html) {
            this.$el.find('.field-validation-error, .field-validation-info').remove();
            this.$el.find('#VideoUri').removeClass('input-validation-error');
            this.$el.find('.video-preview').html('');
            switch (status) {
                case 'success':
                    this.onValidation(null, []);
                    this.$el.find('.video-preview').html(html);
                    break;
                case 'loading':
                    //this.$el.find('#video-uri-field input').after('<div class="field-validation-info"><img src="/img/loader.gif" alt="" />Loading ' + this.provider.getJSON().name + ' video...</div>');
                    break;
                case 'error':
                    this.$el.find('#VideoUri').addClass('input-validation-error');
                    //this.$el.find('#video-uri-field input').after('<div class="field-validation-error">The video link entered is not valid. Please correct the link and try again.</div>');
                    this.onValidation(null, [{ Field: 'VideoUri', Message: 'The video link entered is not valid. Please correct the link and try again.'}]);
                    break;
                default:
                    return;
            }
        },

        onValidation: function (obs, errors) {
            if (errors.length == 0) {
                this.$el.find('.validation-summary').slideUp(function () { $(this).remove(); });
            }

            if (errors.length > 0) {
                if (this.$el.find('.validation-summary').length == 0) {
                    this.$el.find('.video-form').prepend(ich.ValidationSummary({
                        SummaryMessage: 'Please correct the following before continuing:',
                        Errors: errors
                    }));
                    this.$el.find('.validation-summary').slideDown();
                } else {
                    var that = this;
                    // Remove items
                    this.$el.find('.validation-summary li').each(function () {
                        var $li = that.$el.find(this);
                        var found = _.find(errors, function (err) {
                            return 'validation-field-' + err.Field === $li.attr('class');
                        });
                        if (!found) {
                            $li.slideUp(function () { $(this).remove(); });
                        }
                    });

                    // Add items
                    _.each(errors, function (err) {
                        if (this.$el.find('.validation-field-' + err.Field).length === 0) {
                            var li = $('<li class="validation-field-' + err.Field + '">' + err.Message + '</li>').css({ display: 'none' });
                            this.$el.find('.validation-summary ul').append(li);
                            li.slideDown();
                        }
                    }, this);
                }
            }

            if (errors.length == 0) {
                this.$el.find('.add-button').removeAttr('disabled');
            } else {
                this.$el.find('.add-button').attr('disabled', 'disabled');
            }

            this.$el.find('#VideoUri').removeClass('input-validation-error');

            if (_.any(errors, function (item) { return item.Field === 'VideoUri'; })) {
                this.$el.find('#VideoUri').addClass('input-validation-error');
            }
        },

        _onGetVimeoVideo: function (videoId) {
            this._updateVideoStatus('success', '<iframe id="vimeo-video-player" type="text/html" width="520" height="280" src="http://player.vimeo.com/video/' + videoId + '" allowfullscreen></iframe>');
        },

        _onGetYouTubeVideo: function (videoId) {
            this._updateVideoStatus('success', '<iframe id="youtube-video-player" type="text/html" width="520" height="280" src="http://www.youtube.com/embed/' + videoId + '?controls=0&modestbranding=1&rel=0&showinfo=0" allowfullscreen></iframe>');
        },

        _onGetVideoError: function (error) {
            this._updateVideoStatus('error');
        },

        _cancel: function () {
            this.remove();
        },

        _add: function () {
            if (this.videoId !== '') {
                this.trigger('videouploaded', this.videoId, this.provider.getJSON().name);
                this.remove();
            }
        }
    });

    return VideoFormView;
});