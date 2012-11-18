/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// SightingDetailsView
// -------------------

define(['jquery', 'underscore', 'backbone', 'ich', 'app', 'moment', 'timeago', 'tipsy'],
function ($, _, Backbone, ich, app, moment) {

    var SightingDetailsView = Backbone.Marionette.ItemView.extend({

        template: 'SightingFullDetails',

        className: 'observation-details',

        events: {
            'click .thumbnails > div': 'showMedia',
            'click h3 a': 'showSighting'
        },

        initialize: function (options) {
            _.bindAll(this, 'refresh', 'showMedia', 'showNoteForm');
        },

        serializeData: function () {
            var viewModel = this.model.toJSON();
            //viewModel.ShowThumbnails = this.model.get('Media').length > 1 ? true : false;
            //viewModel.ShowProjects = this.model.get('Projects').length > 0 ? true : false;
            viewModel.ObservedOnDescription = moment(this.model.get('ObservedOn')).format('D MMM YYYY h:mma');
            return viewModel;
        },

        currentObservationMedia: null,

        onShow: function () {
            this._showDetails();
            //this.refresh();
        },

        onRender: function () {
            this._showDetails();
            //this.refresh();
        },

        showBootstrappedDetails: function () {
            this._showDetails();
        },

        _showDetails: function () {
            this.currentObservationMedia = _.find(this.model.get('Media'), function (item) {
                return item.IsPrimaryMedia;
            });

            var mapOptions = {
                zoom: 9,
                center: new google.maps.LatLng(this.model.get('Latitude'), this.model.get('Longitude')),
                disableDefaultUI: true,
                scrollwheel: false,
                disableDoubleClickZoom: false,
                draggable: false,
                keyboardShortcuts: false,
                mapTypeId: google.maps.MapTypeId.TERRAIN
            };

            var map = new google.maps.Map(this.$el.find('.map').get(0), mapOptions);
            this.map = map;

            var point = new google.maps.LatLng(Number(this.model.get('Latitude')), Number(this.model.get('Longitude')));
            this.point = point;

            var image = new google.maps.MarkerImage('/img/map-pin.png',
                    new google.maps.Size(43, 38),
                    new google.maps.Point(0, 0)
                );

            var shadow = new google.maps.MarkerImage('/img/map-pin-shadow.png',
                    new google.maps.Size(59, 32),
                    new google.maps.Point(0, 0),
                    new google.maps.Point(17, 32)
                );

            this.mapMarker = new google.maps.Marker({
                position: point,
                map: map,
                clickable: false,
                draggable: false,
                icon: image,
                shadow: shadow
            });

            var resizeTimer;
            var refresh = this.refresh;
            $(window).resize(function () {
                clearTimeout(resizeTimer);
                resizeTimer = setTimeout(function () {
                    refresh();
                }, 100);
            });
        },

        refresh: function () {
            // Resize video and audio in observations
            if (this.currentObservationMedia) { //&& (this.currentObservationMedia.MediaResource.Video || this.currentObservationMedia.MediaResource.Audio)) {
                var newWidth = (600 / 800) * this.$el.find('.preview').width();
                this.$el.find('.preview .video-media > iframe, .preview .audio-media.media-constrained-600, .preview .image-media').height(newWidth + 'px');
            }

            // Resize maps in sightings
            this.map.panTo(this.point);
            this.$el.find('.map').width(this.$el.find('.location').width() + 'px');
            google.maps.event.trigger(this.map, 'resize');
            this.map.panTo(this.point);
        },

        showMedia: function (e) {
            var index = this.$el.find('.thumbnails > div').index(e.currentTarget);
            this.currentObservationMedia = this.model.get('Media')[index];
            var descriptionHtml = '';
            this.$el.find('.preview').empty().append(ich.MediaConstrained600(this.currentObservationMedia.MediaResource));
            if (this.currentObservationMedia.Description && this.currentObservationMedia.Description !== '') {
                descriptionHtml = '<div class="media-details"><div class="overlay"></div><p class="description">' + this.currentObservationMedia.Description + '</p></div>';
                this.$el.find('.preview').append(descriptionHtml);
            }
            this.refresh();
        },

        showNoteForm: function (e) {
            e.preventDefault();
            this.$el.find('.observation-action-menu a').tipsy.revalidate();
            Backbone.history.navigate($(e.currentTarget).attr('href'), { trigger: true });
        },
        
        showSighting: function (e) {
            e.preventDefault();
            Backbone.history.navigate($(e.currentTarget).attr('href'), { trigger: true });
        }
    });

    return SightingDetailsView;

});