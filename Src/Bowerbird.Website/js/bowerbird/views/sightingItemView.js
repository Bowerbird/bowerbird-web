/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/icanhaz/icanhaz.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// SightingItemView
// ----------------

// Shows an individual sighing item
define(['jquery', 'underscore', 'backbone', 'ich', 'app'],
function ($, _, Backbone, ich, app) {

    var SightingItemView = Backbone.Marionette.ItemView.extend({
        tagName: 'li',

        className: 'sighting-item',

//        events: {
//            'click .thumbnails > div': 'showMedia'
//        },

        template: 'SightingItem',

//        currentObservationMedia: null,

//        initialize: function (options) {
//            _.bindAll(this, 'resizeElements', 'showMedia');
//        },

//        serializeData: function () {
//            var json = {
//                Model: {
//                    Activity: this.model.toJSON(),
//                    CreatedDateTimeDescription: moment(this.model.get('CreatedDateTime')).format('D MMM YYYY h:mma')
//                }
//            };
//            if (json.Model.Activity.ObservationAdded) {
//                json.Model.Activity.ObservationAdded.ShowThumbnails = this.model.get('ObservationAdded').Observation.Media.length > 1 ? true : false;
//                json.Model.Activity.ObservationAdded.ShowProjects = this.model.get('ObservationAdded').Observation.Projects.length > 0 ? true : false;
//                json.Model.Activity.ObservationAdded.ObservedOnDescription = moment(this.model.get('ObservationAdded').Observation.ObservedOn).format('D MMM YYYY h:mma');
//            }

//            return json;
//        },

//        onRender: function () {
//            this.$el.find('.time-description').timeago();

//            if (this.model.get('Type') == "observationadded") {
//                this.$el.find('h2 a').on('click', function (e) {
//                    e.preventDefault();
//                    //app.observationRouter.navigate($(this).attr('href'), { trigger: true });
//                    Backbone.history.navigate($(this).attr('href'), { trigger: true });
//                    return false;
//                });

//                this.currentObservationMedia = _.find(this.model.get('ObservationAdded').Observation.Media, function (item) {
//                    return item.IsPrimaryMedia;
//                });

//                var mapOptions = {
//                    zoom: 9,
//                    center: new google.maps.LatLng(-33, 151),
//                    disableDefaultUI: true,
//                    scrollwheel: false,
//                    disableDoubleClickZoom: false,
//                    draggable: false,
//                    keyboardShortcuts: false,
//                    mapTypeId: google.maps.MapTypeId.TERRAIN
//                };

//                var map = new google.maps.Map(this.$el.find('.map').get(0), mapOptions);
//                this.map = map;

//                var point = new google.maps.LatLng(this.model.get('ObservationAdded').Observation.Latitude, this.model.get('ObservationAdded').Observation.Longitude);
//                this.point = point;

//                var image = new google.maps.MarkerImage('http://maps.gstatic.com/mapfiles/ms/icons/blue-dot.png',
//                    new google.maps.Size(32, 32),
//                    new google.maps.Point(0, 0),
//                    new google.maps.Point(15, 32)
//                );

//                var shadow = new google.maps.MarkerImage("http://maps.gstatic.com/mapfiles/kml/paddle/A_maps.shadow.png",
//                    new google.maps.Size(59, 32),
//                    new google.maps.Point(0, 0),
//                    new google.maps.Point(15, 32)
//                );

//                var mapMarker = new google.maps.Marker({
//                    position: point,
//                    map: map,
//                    clickable: false,
//                    draggable: false,
//                    icon: image,
//                    shadow: shadow
//                });

//                var resizeTimer;
//                var resizeElements = this.resizeElements;
//                $(window).resize(function () {
//                    clearTimeout(resizeTimer);
//                    resizeTimer = setTimeout(function () {
//                        resizeElements();
//                    }, 100);
//                });
//            }

//            if (this.model.get('Type') == "postadded") {
//                this.$el.find('h2 a').on('click', function (e) {
//                    e.preventDefault();
//                    app.postRouter.navigate($(this).attr('href'), { trigger: true });

//                    return false;
//                });
//            }

//            return this;
//        },

//        resizeElements: function () {
//            // Resize video and audio in observations
//            if (this.currentObservationMedia) { //&& (this.currentObservationMedia.MediaResource.Video || this.currentObservationMedia.MediaResource.Audio)) {
//                var newWidth = (600 / 800) * this.$el.find('.preview').width();
//                this.$el.find('.preview .video-media > iframe, .preview .audio-media.media-constrained-600, .preview .image-media').height(newWidth + 'px');
//            }

//            // Resize maps in sightings
//            this.$el.find('.map').width(this.$el.find('.location').width() + 'px');
//            google.maps.event.trigger(this.map, 'resize');
//            this.map.panTo(this.point);
//        },

//        showMedia: function (e) {
//            var index = this.$el.find('.thumbnails > div').index(e.currentTarget);
//            this.currentObservationMedia = this.model.get('ObservationAdded').Observation.Media[index];
//            var descriptionHtml = '';
//            this.$el.find('.preview').empty().append(ich.MediaConstrained600(this.currentObservationMedia.MediaResource));
//            if (this.currentObservationMedia.Description && this.currentObservationMedia.Description !== '') {
//                descriptionHtml = '<div class="media-details"><div class="overlay"></div><p class="description">' + this.currentObservationMedia.Description + '</p></div>';
//                this.$el.find('.preview').append(descriptionHtml);
//            }            
//            this.resizeElements();
//        },

        start: function () {
//            if (this.model.get('Type') == "observationadded") {
//                this.resizeElements();
//                this.map.panTo(this.point);
//            }
        }
    });

    return SightingItemView;
});