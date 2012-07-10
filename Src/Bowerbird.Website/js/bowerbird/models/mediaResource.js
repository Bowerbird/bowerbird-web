/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// MediaResource
// -------------

define(['jquery', 'underscore', 'backbone'], function ($, _, Backbone) {

    var MediaResource = Backbone.Model.extend({
        defaults: {
            Key: 0,
            MediumImageUri: '/img/image-upload.png',
            MediaType: 'image',
            Preview: '',
            Description: '',
            ValidVideo: false,
            VisiblePreview: false
        },

        idAttribute: 'Id',

        urlRoot: '/mediaresourceupload',

        initialize: function (options) {
            log('EmbeddedVideo.initialize', options);
            _.bindAll(this, 'previewVideo');
        },

        // check that we have either a youtube link or a vimeo link.
        // if success, create the embed script from the link
        // if failure, return error message
        previewVideo: function (linkUri) {
            log('EmbeddedVideo.previewVideo', linkUri);
            var that = this;
            $.when(this.getVideoPreview(linkUri))
            .done(function (data) {
                log(data);
                that.set('Preview', data.PreviewTags);
                that.set('ValidVideo', data.success);
            });

            this.set('LinkUri', linkUri);
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

    return MediaResource;

});