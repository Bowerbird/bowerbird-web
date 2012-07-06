/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// EmbeddedVideo
// -------------

define(['jquery', 'underscore', 'backbone'],
function ($, _, Backbone) {
    var EmbeddedVideo = Backbone.Model.extend({
        defaults: {
            LinkUri: '',
            EmbedScript: '',
            Title: '',
            Description: '',
            Provider: '',
            ErrorMessage: '',
            VideoId: '',
            MediaType: 'video'
        },

        urlRoot: '/videoupload',

        idAttribute: 'Id',

        initialize: function (options) {
            log('EmbeddedVideo.initialize', options);
            _.bindAll(this, 'previewVideo', 'setEmbedScript');
        },

        // check that we have either a youtube link or a vimeo link.
        // if success, create the embed script from the link
        // if failure, return error message
        previewVideo: function (linkUri) {
            log('EmbeddedVideo.previewVideo', linkUri);

            //var videoPreviewScript = this.getVideoPreview(linkUri);
            var that = this;
            $.when(this.getVideoPreview(linkUri))
            .done(function (data) {
                log(data);
                //this.setEmbedScript(data);
                that.set('EmbedScript', data.EmbedTags);
            });

            this.set('LinkUri', linkUri);
        },

        setEmbedScript: function (data) {
            log('EmbeddedVideo.setEmbedScript');
            this.set('EmbedScript', data);
        },

        getVideoPreview: function (url) {
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
        }
    });

    return EmbeddedVideo;
});