/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// LocationFormView
// ----------------

// View that allows user to choose location on a mpa or via coordinates
define(['jquery', 'underscore', 'backbone', 'app', 'views/dummyoverlayview', 'jqueryui/autocomplete', 'jqueryui/draggable', 'async!http://maps.google.com/maps/api/js?sensor=false&region=AU'],
function ($, _, Backbone, app, DummyOverlayView) {

    var australia = new google.maps.LatLng(-29.191427, 134.472126); // Centre on Australia

    var ExpandMapControl = function (controlDiv, map, callback) {

        // Set CSS styles for the DIV containing the control
        // Setting padding to 5 px will offset the control
        // from the edge of the map
        controlDiv.style.padding = '5px';

        // Set CSS for the control UI
        var controlUI = document.createElement('div');
        controlUI.title = 'Expand the size of the map';
        controlUI.setAttribute('id', 'expand-map-widget');
        controlUI.innerHTML = 'Expand Map';
        controlDiv.appendChild(controlUI);

        google.maps.event.addDomListener(controlUI, 'click', function () {
            callback();
        });

    };

    var CentreMapControl = function (controlDiv, map, callback) {

        // Set CSS styles for the DIV containing the control
        // Setting padding to 5 px will offset the control
        // from the edge of the map
        controlDiv.style.padding = '5px';

        // Set CSS for the control UI
        var controlUI = document.createElement('div');
        controlUI.title = 'Centre on marker';
        controlUI.setAttribute('id', 'centre-map-widget');
        controlUI.innerHTML = 'Centre Pin';
        controlDiv.appendChild(controlUI);

        google.maps.event.addDomListener(controlUI, 'click', function () {
            callback();
        });

    };

    var LocationFormView = Backbone.View.extend({
        id: 'location-details',

        initialize: function (options) {
            _.extend(this, Backbone.Events);
            _.bindAll(this, 'onMediaChanged', '_expandMap', '_centreMap');
            this.mapMarker = null;
            this.zIndex = 0;

            // Observations have media, records don't
            if (this.model.media) {
                this.model.media.on('add', this.onMediaChanged, this);
            }
        },

        render: function () {
            this._initMap();
            if (this.model.has('Address')) {
                this._initAddressField();
            }
            this._initLocationPin();

            app.vent.on('view:render:complete', function () {
                var currentCentre = this.map.getCenter();

                google.maps.event.trigger(this.map, 'resize');

                this.map.panTo(currentCentre);
            }, this);

            return this;
        },

        onMediaChanged: function (media) {
            var lat = media.mediaResource.get('Metadata').Latitude;
            var lon = media.mediaResource.get('Metadata').Longitude;
            if ((this.model.get('Latitude') === '' && this.model.get('Longitude') === '') && lat && lon) {
                this.changeMarkerPosition(lat, lon);
            }
        },

        _initMap: function () {
            var g = google.maps;

            var mapSettings = {
                center: australia,
                zoom: 3,
                panControl: false,
                streetViewControl: false,
                mapTypeControl: true,
                scrollwheel: true,
                mapTypeId: g.MapTypeId.TERRAIN
            };

            this.map = new g.Map(document.getElementById("location-map"), mapSettings);
            this.iw = new g.InfoWindow();
            this.geocoder = new g.Geocoder();

            // Add a dummy overlay for later use.
            // Needed for API v3 to convert pixels to latlng.
            this.dummy = new DummyOverlayView(this.map);

            // Create the DIV to hold the control and
            // call the ExpandMapControl() constructor passing
            // in this DIV.
            var expandMapControlDiv = document.createElement('div');
            var expandMapControl = new ExpandMapControl(expandMapControlDiv, this.map, this._expandMap);
            expandMapControlDiv.index = 2;
            this.map.controls[g.ControlPosition.TOP_RIGHT].push(expandMapControlDiv);

            // Create the DIV to hold the control and
            // call the CentreMapControl() constructor passing
            // in this DIV.
            var centreMapControlDiv = document.createElement('div');
            var centreMapControl = new CentreMapControl(centreMapControlDiv, this.map, this._centreMap);
            centreMapControlDiv.index = 1;
            this.map.controls[g.ControlPosition.TOP_RIGHT].push(centreMapControlDiv);

            this.mapExpanded = false;
        },

        _expandMap: function () {
            if (this.mapExpanded) {
                this.$el.find('#location-map').css('height', '242px');
                this.mapExpanded = false;
                this.$el.find('#expand-map-widget').text('Expand Map');
            } else {
                this.$el.find('#location-map').css('height', '600px');
                this.mapExpanded = true;
                this.$el.find('#expand-map-widget').text('Shrink Map');
            }

            var currentCentre = this.map.getCenter();

            google.maps.event.trigger(this.map, 'resize');

            this.map.panTo(currentCentre);
        },

        _centreMap: function () {
            if (this.mapMarker) {
                this.map.panTo(this.mapMarker.position);
            }
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
                            };
                        }));
                    });
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

        inMap: false,

        _initLocationPin: function () {
            var self = this;
            $("#location-pin").draggable({
                //                drag: function (event, ui) {
                //                        var locationPinLeft = ui.helper.offset().left + ui.helper.width() / 2;
                //                        var locationPinTop = ui.helper.offset().top + ui.helper.height() / 2;
                //                        $('#debug-info').text('left: ' + locationPinLeft + ', top: ' + locationPinTop);
                //                },
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
                        self.inMap = true;
                        // Find the object's pixel position in the map container
                        var g = google.maps;
                        var pixelpoint = new g.Point(locationX - mapDivLeft, locationY - mapDivTop + (locationPinHeight / 2));

                        // Corresponding geo point on the map
                        var proj = self.dummy.getProjection();
                        var latlng = proj.fromContainerPixelToLatLng(pixelpoint);

                        // Create a corresponding marker on the map
                        self._positionMarker(latlng);
                        self._reverseGeocode();

                        //self._removeMarkerFromPlaceholder();
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

            var image = new g.MarkerImage('/img/map-pin.png',
                new g.Size(43, 38),
                new g.Point(0, 0)
            );

            var shadow = new g.MarkerImage('/img/map-pin-shadow.png',
                new g.Size(59, 32),
                new g.Point(0, 0),
                new g.Point(17, 32)
                );

            this.mapMarker = new g.Marker({
                position: point,
                map: this.map,
                clickable: true,
                draggable: true,
                raiseOnDrag: false,
                icon: image,
                shadow: shadow,
                optimized: false,
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

            this._removeMarkerFromPlaceholder();
        },

        _highestOrder: function () {
            /**
            * The currently dragged marker on the map
            * always gets the highest z-index too
            */
            return this.zIndex;
        },

        changeMarkerPosition: function (lat, lng) {
            var latlng = new google.maps.LatLng(lat, lng);

            if (this.mapMarker === null) {
                this._positionMarker(latlng);
                this._removeMarkerFromPlaceholder();
                this._reverseGeocode();
            }
            else {
                this._positionMarker(latlng);
                this._reverseGeocode();
            }

            var position = this.mapMarker.getPosition();
            var newPoint = new google.maps.LatLng(position.lat() + .02, position.lng());
            this.map.panTo(newPoint);
        },

        _displayLatLong: function (fireLatLongFieldsChangeEvent) {
            if (this.mapMarker) {
                var lat = this.mapMarker.getPosition().lat();
                var lng = this.mapMarker.getPosition().lng();

                //            if (this.model.get('anonymiseLocation') === true) {
                $('#Latitude').val(lat);
                $('#Longitude').val(lng);
                this.$el.find('#lat-long').text(lat + ', ' + lng);

                //            }
                //            else {
                //                $('#latitude').val(parseFloat(lat).toFixed(1));
                //                $('#longitude').val(parseFloat(lng).toFixed(1));
                //            }
                if (fireLatLongFieldsChangeEvent || !this.model.has('Address')) {
                    //$('#latitude').change();
                    $('#Longitude').change();
                }
            }
            else {
                return;
            }
        },

        _removeMarkerFromPlaceholder: function () {
            setTimeout(function () {
                $('#location-pin').remove();
            }, 500); // Delay removal until gmaps marker is added
        },

        _reverseGeocode: function () {
            if (this.model.has('Address')) {
                if (this.mapMarker) {
                    this.geocoder.geocode({ latLng: this.mapMarker.getPosition() }, this._reverseGeocodeResult);
                }
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

    return LocationFormView;

});