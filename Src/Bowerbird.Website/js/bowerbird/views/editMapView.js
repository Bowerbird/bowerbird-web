/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// EditMapView
// -----------

// View that allows user to choose location on a mpa or via coordinates
define(['jquery', 'underscore', 'backbone', 'app', 'views/dummyoverlayview', 'jqueryui/autocomplete', 'jqueryui/draggable', 'async!http://maps.google.com/maps/api/js?sensor=false&region=AU'],
function ($, _, Backbone, app, DummyOverlayView) {
    var EditMapView = Backbone.View.extend({
        id: 'location-fieldset',

        initialize: function (options) {
            this.observation = options.model;
            this.mapMarker = null;
            this.zIndex = 0;

            this.observation.mediaResources.on('change:Metadata', this.onMediaResourceFilesChanged, this);
        },

        render: function () {
            this._initMap();
            this._initAddressField();
            this._initLocationPin();
            return this;
        },

        onMediaResourceFilesChanged: function (mediaResource) {
            var lat = mediaResource.get('Metadata').Latitude;
            var lon = mediaResource.get('Metadata').Longitude;

            if ((this.observation.get('Latitude') === null && this.observation.get('Longitude') === null) && lat && lon) {
                this.changeMarkerPosition(lat, lon);
            }
        },

        _initMap: function () {
            var g = google.maps;

            var mapSettings = {
                center: new g.LatLng(-29.191427, 134.472126), // Centre on Australia
                zoom: 4,
                panControl: false,
                streetViewControl: false,
                mapTypeControl: true,
                scrollwheel: false,
                mapTypeId: g.MapTypeId.TERRAIN
            };

            this.map = new g.Map(document.getElementById("location-map"), mapSettings);
            this.iw = new g.InfoWindow();
            this.geocoder = new g.Geocoder();

            // Add a dummy overlay for later use.
            // Needed for API v3 to convert pixels to latlng.
            this.dummy = new DummyOverlayView(this.map);
        },

        _initAddressField: function () {
            var self = this;
            // http://tech.cibul.net/geocode-with-google-maps-api-v3/
            this.$el.find("#Address").autocomplete({
                //This bit uses the geocoder to fetch address values
                source: function (request, response) {
                    self.geocoder.geocode({ address: request.term, region: 'AU' }, function (results, status) {
                        response($.map(results, function (item) {
                            return {
                                label: item.formatted_address,
                                value: item.formatted_address,
                                latitude: item.geometry.location.lat(),
                                longitude: item.geometry.location.lng()
                            }
                        }));
                    })
                },
                //This bit is executed upon selection of an address
                select: function (event, ui) {
                    var location = new google.maps.LatLng(ui.item.latitude, ui.item.longitude);
                    self._positionMarker(location);
                    self._removeMarkerFromPlaceholder();
                    self._reverseGeocode();

                    var position = self.mapMarker.getPosition();
                    var newPoint = new google.maps.LatLng(position.lat() + .02, position.lng());
                    self.map.panTo(newPoint);
                }
            });
        },

        _initLocationPin: function () {
            var self = this;
            $("#location-pin").draggable({
                //            drag: function (event, ui) {
                //                var locationPinLeft = ui.helper.offset().left + ui.helper.width() / 2;
                //                var locationPinTop = ui.helper.offset().top + ui.helper.height() / 2;
                //                $('#debug-info').text('left: ' + locationPinLeft + ', top: ' + locationPinTop);
                //            },
                stop: function (event, ui) {
                    var $mapDiv = $(self.map.getDiv());

                    var mapDivLeft = $mapDiv.offset().left;
                    var mapDivTop = $mapDiv.offset().top;
                    var mapDivWidth = $mapDiv.width();
                    var mapDivHeight = $mapDiv.height();

                    var locationPinLeft = ui.helper.offset().left;
                    var locationPinTop = ui.helper.offset().top;
                    var locationPinWidth = ui.helper.width();
                    var locationPinHeight = ui.helper.height();

                    var locationX = locationPinLeft + (locationPinWidth / 2);
                    var locationY = locationPinTop + (locationPinHeight / 2) - 6;

                    // Check if the cursor is inside the map div
                    if (locationX > mapDivLeft && locationX < (mapDivLeft + mapDivWidth) && locationY > mapDivTop && locationY < (mapDivTop + mapDivHeight)) {
                        // Find the object's pixel position in the map container
                        var g = google.maps;
                        var pixelpoint = new g.Point(locationX - mapDivLeft, locationY - mapDivTop + (locationPinHeight / 2));

                        // Corresponding geo point on the map
                        var proj = self.dummy.getProjection();
                        var latlng = proj.fromContainerPixelToLatLng(pixelpoint);

                        // Create a corresponding marker on the map
                        self._positionMarker(latlng);
                        self._removeMarkerFromPlaceholder();
                        self._reverseGeocode();
                    }
                }
            });
        },

        _positionMarker: function (point) {
            // Remove map marker first
            if (this.mapMarker) {
                this.mapMarker.setMap(null);
                this.mapMarker = null;
            }

            var g = google.maps;

            var image = new g.MarkerImage('http://maps.gstatic.com/mapfiles/ms/icons/blue-dot.png',
                new g.Size(32, 32),
                new g.Point(0, 0),
                new g.Point(15, 32)
                );

            var shadow = new g.MarkerImage("http://maps.gstatic.com/mapfiles/kml/paddle/A_maps.shadow.png",
                new g.Size(59, 32),
                new g.Point(0, 0),
                new g.Point(15, 32)
                );

            this.mapMarker = new g.Marker({
                position: point,
                map: this.map,
                clickable: true,
                draggable: true,
                raiseOnDrag: false,
                icon: image,
                shadow: shadow,
                zIndex: this._highestOrder()
            });

            var self = this;

            g.event.addListener(this.mapMarker, "drag", function () {
                //        var lat = mapMarker.getPosition().lat();
                //        var lng = mapMarker.getPosition().lng();
                //        iw.setContent(lat.toFixed(6) + ", " + lng.toFixed(6));
                //        iw.open(map, this);
                self._displayLatLong(false);
            });

            g.event.addListener(this.mapMarker, "dragstart", function () {
                // Close infowindow when dragging the marker whose infowindow is open
                //if (actual == this.mapMarker) iw.close();
                // Increment z_index
                self.zIndex++;
                self.mapMarker.setZIndex(self._highestOrder());
            });

            g.event.addListener(this.mapMarker, "dragend", function () {
                self._displayLatLong(true);
                self._reverseGeocode();
            });

            this._displayLatLong();
        },

        _highestOrder: function () {
            /**
            * The currently dragged marker on the map
            * always gets the highest z-index too
            */
            return this.zIndex;
        },

        changeMarkerPosition: function (lat, long) {
            var latlng = new google.maps.LatLng(lat, long);

            if (this.mapMarker === null) {
                this._positionMarker(latlng);
                this._removeMarkerFromPlaceholder();
                this._reverseGeocode();
            }
            else {
                this._positionMarker(latlng);
                this._reverseGeocode();
            }

            if (!this.map.getBounds().contains(this.mapMarker.getPosition())) {
                var position = this.mapMarker.getPosition();
                var newPoint = new google.maps.LatLng(position.lat() + .02, position.lng());
                this.map.panTo(newPoint);
            }
        },

        _displayLatLong: function (fireLatLongFieldsChangeEvent) {
            if (this.mapMarker) {
                var lat = this.mapMarker.getPosition().lat();
                var lng = this.mapMarker.getPosition().lng();
                //            if (this.observation.get('anonymiseLocation') === true) {
                $('#Latitude').val(lat);
                $('#Longitude').val(lng);
                //            }
                //            else {
                //                $('#latitude').val(parseFloat(lat).toFixed(1));
                //                $('#longitude').val(parseFloat(lng).toFixed(1));
                //            }
                if (fireLatLongFieldsChangeEvent) {
                    //$('#latitude').change();
                    $('#Longitude').change();
                }
            }
            else {
                return;
            }
        },

        _removeMarkerFromPlaceholder: function () {
            $('#location-pin').remove();
        },

        _reverseGeocode: function () {
            if (this.mapMarker) {
                this.geocoder.geocode({ latLng: this.mapMarker.getPosition() }, this._reverseGeocodeResult);
            }
        },

        _reverseGeocodeResult: function (results, status) {
            if (status == 'OK') {
                if (results.length == 0) {
                    $('#Address').val('');
                } else {
                    var addressResult = results[0].formatted_address;
                    $('#Address').val(addressResult);
                }
            } else {
                $('#Address').val('');
            }
            $('#Address').change();
        }
    });

    return EditMapView;

});