/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// MediaResourceItemView
// ---------------------

define(['jquery', 'underscore', 'backbone', 'app', 'ich'],
function ($, _, Backbone, app, ich) {
    var ImageProvider = function (options) {

    };

    var VideoProvider = function (options) {

    };

    var MediaResourceItemView = Backbone.View.extend({
        className: 'media-resource-item',

        events: {
            'click .view-media-resource-button': 'viewMediaResource',
            'click .add-caption-button': 'viewMediaResource',
            'click .remove-media-resource-button': 'removeMediaResource'
        },

        provider: null,

        initialize: function (options) {
            _.extend(this, Backbone.Events);

            var mediaType = this.model.get('MediaType');
            if (mediaType === 'image') {
                this.provider = new ImageProvider();
            } else if (mediaType === 'video') {
                this.provider = new VideoProvider();
            }
        },

        render: function () {
            this.$el
                .append(ich.ObservationMediaResourceItem(this.model.toJSON())).css({ position: 'absolute', top: '-250px' })
                .css({ width: 280 + 'px' });
            return this;
        },

        viewMediaResource: function () {
            alert('Coming soon');
        },

        removeMediaResource: function () {
            this.trigger('mediaresourceview:remove', this.model, this);
        }
    });

    return MediaResourceItemView;

});