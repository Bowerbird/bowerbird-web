/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// EmbeddedVideo
// -------------

define(['jquery', 'underscore', 'backbone'], function ($, _, Backbone) {

    var EmbeddedVideo = Backbone.Model.extend({
        defaults: {
            LinkUri: '',
            EmbedScript: '',
            Title: '',
            Description: '',
            Provider: '',
            ErrorMessage: '',
            VideoId: ''
        },

        idAttribute: 'Id',

        initialize: function (options) {
            log('EmbeddedVideo.initialize', options);
        },

        // check that we have either a youtube link or a vimeo link.
        // if success, create the embed script from the link
        // if failure, return error message
        validateLink: function (linkUri) {
            log('EmbeddedVideo.validateLink', linkUri);
            if (!(this.isYoutube(linkUri) || this.isVimeo(linkUri))) {
                this.ErrorMessage = "Link is not a Youtube or Vimeo link";
                return false;
            }
            this.LinkUri = linkUri;
            this.createEmbedScriptFromLink(this.extractVideoId(linkUri));
            return true;
        },

        // detect if the link has youtube patterns
        isYoutube: function (text) {
            var isYoutube1 = /www.youtube.com/i;
            var isYoutube2 = /youtu.be/i;
            return isYoutube1.test(text) || isYoutube2.test(text);
        },

        // detect if the link has vimeo patterns
        isVimeo: function (text) {
            var isVimeo1 = /vimeo.com/i;
            return isVimeo1.test(text)
        },

        extractVideoId: function (text) {
            var idString = text.substr(text.lastIndexOf("/") + 1, text.length - text.lastIndexOf("/"));
            if (idString.indexOf("watch?v=") > -1) {
                this.VideoId = idString.split("=")[1];
            } else {
                this.VideoId = idString;
            }
        },

        // create the embed script from the link
        createEmbedScriptFromLink: function () {
            var linkUri = this.LinkUri;
            var videoId = this.VideoId;
            var embedScript = "";
            if (this.isYoutube(linkUri)) {
                embedScript = '<iframe width=&quot;560&quot; height=&quot;315&quot; src=&quot;http://www.youtube.com/embed/' + videoId + '&quot; frameborder=&quot;0&quot; allowfullscreen></iframe>';
            }
            else if (this.isVimeo(linkUri)) {
                embedScript = '<iframe src=&quot;http://player.vimeo.com/video/' + videoId + '&quot; width=&quot;500&quot; height=&quot;281&quot; frameborder=&quot;0&quot; allowFullScreen></iframe>';
            }
            this.EmbedScript = embedScript;
        }
    });

    return EmbeddedVideo;
});