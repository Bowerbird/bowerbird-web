/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ObservationMedia
// ----------------

define(['jquery', 'underscore', 'backbone'], function ($, _, Backbone) {

    var ObservationMedia = Backbone.Model.extend({
        defaults: {
            MediaResourceId: '',
            Description: '',
            Licence: 'BY-SA',
            IsPrimaryMedia: false
        },

        mediaResource: null,

        initialize: function (attributes, options) {
            if (options.mediaResource) {
                this.setMediaResource(options.mediaResource);
            }
        },

        setMediaResource: function (mediaResource) {
            this.mediaResource = mediaResource;
            this.set('MediaResourceId', mediaResource.id);

            if (mediaResource.get('Metadata').Description)
            {
                this.set('Description', mediaResource.get('Metadata').Description);
            }
    }
    });

    return ObservationMedia;

});