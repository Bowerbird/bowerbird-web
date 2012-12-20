/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// UserDetailsView
// ---------------

define(['jquery', 'underscore', 'backbone', 'ich', 'app', 'moment', 'timeago', 'tipsy'],
function ($, _, Backbone, ich, app, moment) {

    var UserDetailsView = Backbone.Marionette.ItemView.extend({

        className: 'user-details',
        
        template: 'UserTileDetails',

        events: {
            //'click .thumbnails > div': 'showMedia',
            'click .view-button': 'showItem',
            'click h3 a': 'showItem'
        },

        initialize: function (options) {
            _.bindAll(this, 'refresh');
        },

        serializeData: function () {
            var viewModel = this.model.toJSON();
            return viewModel;
        },

        onShow: function () {
            this._showDetails();
        },

        onRender: function () {
            this._showDetails();
        },

        showBootstrappedDetails: function () {
            this._showDetails();
        },

        _showDetails: function () {
            var resizeTimer;
            var refresh = this.refresh;
            $(window).resize(function () {
                clearTimeout(resizeTimer);
                resizeTimer = setTimeout(function () {
                    refresh();
                }, 100);
            });

            this.$el.find('.actions .button').tipsy({ gravity: 's', html: true });
        },

        refresh: function () {
//            // Resize video and audio in observations
//            if (this.currentObservationMedia) {
//                var newWidth = (600 / 800) * this.$el.find('.preview').width();
//                this.$el.find('.preview .video-media, .preview .video-media > iframe, .preview .audio-media.media-constrained-600, .preview .image-media').height(newWidth + 'px');
//            }

//            if (this.showLocation) {
//                // Resize maps in sightings
//                this.map.panTo(this.point);
//                this.$el.find('.map').width(this.$el.find('.location').width() + 'px');
//                google.maps.event.trigger(this.map, 'resize');
//                this.map.panTo(this.point);
//            }
        },

        showMedia: function (e) {
//            var index = this.$el.find('.thumbnails > div').index(e.currentTarget);
//            this.currentObservationMedia = this.model.get('Media')[index];
//            var descriptionHtml = '';
//            this.$el.find('.preview').empty().append(ich.MediaConstrained600(this.currentObservationMedia.MediaResource));
//            if (this.currentObservationMedia.Description && this.currentObservationMedia.Description !== '') {
//                descriptionHtml = '<div class="media-details"><div class="overlay"></div><p class="description">' + this.currentObservationMedia.Description + '</p></div>';
//                this.$el.find('.preview').append(descriptionHtml);
//            }
            this.refresh();
        },

//        showNoteForm: function (e) {
//            e.preventDefault();
//            this.$el.find('.observation-action-menu a').tipsy.revalidate();
//            Backbone.history.navigate($(e.currentTarget).attr('href'), { trigger: true });
//        },
        
//        showNoteForm: function (e) {
//            e.preventDefault();
//            this.$el.find('.actions a').tipsy.revalidate();
//            Backbone.history.navigate($(e.currentTarget).attr('href'), { trigger: true });
//        },

        showItem: function (e) {
            e.preventDefault();
            this.$el.find('.actions a').tipsy.revalidate();
            Backbone.history.navigate($(e.currentTarget).attr('href'), { trigger: true });
        }
    });

    return UserDetailsView;

});