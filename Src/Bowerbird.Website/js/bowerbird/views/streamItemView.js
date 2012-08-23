/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// StreamItemView
// --------------

// Shows an individual stream item
define(['jquery', 'underscore', 'backbone', 'app', 'moment', 'timeago'],
function ($, _, Backbone, app, moment) {

    var StreamItemView = Backbone.Marionette.ItemView.extend({
        tagName: 'li',

        className: 'stream-item observation-stream-item',

        template: 'StreamItem',

        serializeData: function () {
            var json = {
                Model: {
                    Activity: this.model.toJSON(),
                    CreatedDateTimeDescription: moment(this.model.get('CreatedDateTime')).format('D MMM YYYY h:mma')
                }
            };
            if (json.Model.Activity.ObservationAdded) {
                json.Model.Activity.ObservationAdded.ShowThumbnails = this.model.get('ObservationAdded').Observation.Media.length > 1 ? true : false;
                json.Model.Activity.ObservationAdded.ShowProjects = this.model.get('ObservationAdded').Observation.Projects.length > 0 ? true : false;
                json.Model.Activity.ObservationAdded.ObservedOnDescription = moment(this.model.get('ObservationAdded').Observation.ObservedOn).format('D MMM YYYY h:mma');
            }

            return json;
        },

        onRender: function () {
            this.$el.find('.time-description').timeago();

            if (this.model.get('Type') == "observationadded") {
                this.$el.find('h2 a').on('click', function (e) {
                    e.preventDefault();
                    app.observationRouter.navigate($(this).attr('href'), { trigger: true });
                    return false;
                });

                var mapOptions = {
                    zoom: 9,
                    center: new google.maps.LatLng(-33, 151),
                    disableDefaultUI: true,
                    scrollwheel: false,
                    disableDoubleClickZoom: false,
                    draggable: false,
                    keyboardShortcuts: false,
                    mapTypeId: google.maps.MapTypeId.TERRAIN
                }

                var map = new google.maps.Map(this.$el.find('.map').get(0), mapOptions);
                this.map = map;

                var point = new google.maps.LatLng(this.model.get('ObservationAdded').Observation.Latitude, this.model.get('ObservationAdded').Observation.Longitude);
                this.point = point;

                var image = new google.maps.MarkerImage('http://maps.gstatic.com/mapfiles/ms/icons/blue-dot.png',
                    new google.maps.Size(32, 32),
                    new google.maps.Point(0, 0),
                    new google.maps.Point(15, 32)
                );

                var shadow = new google.maps.MarkerImage("http://maps.gstatic.com/mapfiles/kml/paddle/A_maps.shadow.png",
                    new google.maps.Size(59, 32),
                    new google.maps.Point(0, 0),
                    new google.maps.Point(15, 32)
                );

                var mapMarker = new google.maps.Marker({
                    position: point,
                    map: map,
                    clickable: false,
                    draggable: false,
                    icon: image,
                    shadow: shadow
                });

                //var newPoint = new google.maps.LatLng(point.lat(), point.lng());

                var $map = this.$el.find('.map');
                var $location = this.$el.find('.location');
                var resizeTimer;
                $(window).resize(function () {
                    clearTimeout(resizeTimer);
                    resizeTimer = setTimeout(function () {
                        $map.width($location.width() + 'px');
                        google.maps.event.trigger(map, 'resize');
                        map.panTo(point);
                    }, 100);
                });

                //                log('width', $location.width());
                //                $map.width($location.width() + 'px')
                //                google.maps.event.trigger(map, 'resize');
                //map.panTo(point);
            }

            if (this.model.get('Type') == "postadded") {
                this.$el.find('h2 a').on('click', function (e) {
                    e.preventDefault();
                    app.postRouter.navigate($(this).attr('href'), { trigger: true });

                    return false;
                });
            }

            return this;
        },

        start: function () {
            if (this.model.get('Type') == "observationadded") {
                var $map = this.$el.find('.map');
                var $location = this.$el.find('.location');

                $map.width($location.width() + 'px')
                google.maps.event.trigger(this.map, 'resize');
                this.map.panTo(this.point);
            }
        }
    });

    return StreamItemView;
});