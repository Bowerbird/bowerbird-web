/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// VideoForm
// ---------

// Shows an individual project item
define(['jquery', 'underscore', 'backbone', 'app', 'ich'],
function ($, _, Backbone, app, ich) {
    var VideoForm = Backbone.Marionette.ItemView.extend({

        id: 'video-form',

        template: 'VideoForm',

        events: {
            //'click button#embed-video-clear-button': '_clear',
            'click .cancel-button': '_cancel',
            //'click button#embed-video-next-button': '_next',
            'change input#VideoUri': '_updateLink',
            //'change textarea#embed-video-description-input': '_updateDescription',
            'click .close': '_cancel'
        },

        initialize: function (options) {
            //log('embeddedVideoView:initialize', options);
            //this.ValidVideo = false;
            //this.VisiblePreview = false;
            //this.Preview = '';
            this.model.set('MediaType', 'video');
            //this.model.on('change:Metadata', this.onMediaResourceFilesChanged, this);
            this.model.on('change:Files', this.onMediaResourceFilesChanged, this);
        },

        onRender: function () {
            //log('embeddedVideoView:onRender');
            //this._resetView();
            this._showElement($('#modal-dialog'));
            //this._hideElement($('div#embed-video-preview'));
            return this;
        },

        // fire this event when the link has been updated
        _updateLink: function () {
            //log('embeddedVideoView:_updateLink');
            this._previewVideo($('input#VideoUri').val());
        },

        _previewVideo: function (videoUri) {
            this._getYoutubeVideo(videoUri);
            //log('EmbeddedVideo.previewVideo', videoUri);
            //            var that = this;
            //            $.when(this._getVideoPreview(videoUri))
            //                .done(function (data) {
            //                    log(data, data.PreviewTags, data.success);
            //                    if (data.success) {
            //                        that._next(data.PreviewTags);
            //                    }
            //                    //that.Preview = data.PreviewTags;
            //                    //that.ValidVideo = data.success;
            //                });

            //            this.model.set('VideoUri', videoUri);
        },

        _getVideoPreview: function (url) {
            log('EmbeddedVideo.getVideoPreview');
            var deferred = new $.Deferred();
            var params = {};
            params['url'] = url;
            $.ajax({
                url: '/mediaresources/videopreview',
                type: "POST",
                data: params
            }).done(function (data) {
                deferred.resolve(data);
            });
            return deferred.promise();
        },

        _getYoutubeVideo: function (value) {
            var videoId = '';

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
                videoId = uriMatch[2];
            } else {
                /*
                Handles actual Id being entered
                */
                var idRegExp = /^[A-Za-z0-9_\-]{8,32}$/;
                var idMatch = value.match(idRegExp);
                if (idMatch && idMatch == true) {
                    videoId = value;
                } else {
                    //error
                }
            }

            var that = this;
            $.ajax({
                type: "GET",
                dataType: "jsonp",
                url: 'http://gdata.youtube.com/feeds/api/videos/' + encodeURIComponent(videoId) + '?v=2&alt=json-in-script',
                success: function (obj) {
                    log('youtube video data', obj);
                    that._youtubeFetchDataCallback(obj);
                }
            });


            //            if (/^https?\:\/\/.+/i.test(tempvar)) {
            //                tempvar = /[\?\&]v=([^\?\&]+)/.exec(tempvar);
            //                if (!tempvar) {
            //                    alert('YouTube video URL has a problem!');
            //                    return;
            //                }
            //                videoid = tempvar[1];
            //            }
            //            else {
            //                if (/^[A-Za-z0-9_\-]{8,32}$/.test(tempvar) == false) {
            //                    alert('YouTube video ID has a problem!');
            //                    return;
            //                }
            //                videoid = tempvar;
            //            }
            //$.getScript('http://gdata.youtube.com/feeds/api/videos/' + encodeURIComponent(videoId) + '?v=2&alt=json-in-script&callback=_youtubeFetchDataCallback');

        },

        _youtubeFetchDataCallback: function (data) {
            var s = '';
            s += '<img src="' + data.entry["media$group"]["media$thumbnail"][0].url + '" width="' + data.entry["media$group"]["media$thumbnail"][0].width + '" height="' + data.entry["media$group"]["media$thumbnail"][0].height + '" alt="Default Thumbnail" align="right"/>';
            s += '<b>Title:</b> ' + data.entry["title"].$t + '<br/>';
            s += '<b>Author:</b> ' + data.entry["author"][0].name.$t + '<br/>';
            s += '<b>Published:</b> ' + new Date(data.entry["published"].$t.substr(0, 4), data.entry["published"].$t.substr(5, 2) - 1, data.entry["published"].$t.substr(8, 2)).toLocaleDateString() + '<br/>';
            s += '<b>Duration:</b> ' + Math.floor(data.entry["media$group"]["yt$duration"].seconds / 60) + ':' + (data.entry["media$group"]["yt$duration"].seconds % 60) + ' (' + data.entry["media$group"]["yt$duration"].seconds + ' seconds)<br/>';
            s += '<b>Rating:</b> ' + new Number(data.entry["gd$rating"].average).toFixed(1) + ' out of ' + data.entry["gd$rating"].max + '; ' + data.entry["gd$rating"].numRaters + ' rating(s)' + '<br/>';
            s += '<b>Statistics:</b> ' + data.entry["yt$statistics"].favoriteCount + ' favorite(s); ' + data.entry["yt$statistics"].viewCount + ' view(s)' + '<br/>';
            s += '<br/>' + data.entry["media$group"]["media$description"].$t.replace(/\n/g, '<br/>') + '<br/>';
            s += '<br/><a href="' + data.entry["media$group"]["media$player"].url + '" target="_blank">Watch on YouTube</a>';
            //$('#youtubeDataFetcherOutput').html(s);
            alert(s);
        },

        //        // Fire this event when the description has been updated
        //        _updateDescription: function () {
        //            this.model.set('Description', $('input#embed-video-description-input').val());
        //        },

        //        // remove the current video preview and reset the form..
        //        _clear: function () {
        //            this._resetView();
        //        },

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
        _next: function (preview) {
            log('embeddedVideoView:_next');
            // if we've got a valid video, and we've seen it, we are clicking to save
            //            if (this.ValidVideo && this.VisiblePreview) {
            //                this._save();
            //            }
            // if we've loaded a new video, but haven't seen it, display it:
            //else if (this.ValidVideo) {
            //var src = this.Preview;
            //this._showElement($('div#embed-video-preview'));
            this.$el.find('.video-preview').empty().append(ich.VideoPreview({ Width: 520, Height: 400, Source: preview }));
            // now we've seen it, so next click we want to save it.
            //this.VisiblePreview = true;
            //}
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

    return VideoForm;
});