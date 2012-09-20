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

        // Setup the click event listeners: simply set the map to australia
        //        google.maps.event.addDomListener(controlUI, 'click', function () {
        //            map.setCenter(australia);
        //        });

        google.maps.event.addDomListener(controlUI, 'click', function () {
            callback();
        });

    };

    var EditMapView = Backbone.View.extend({
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
                zoom: 4,
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
            $('#location-pin').remove();
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

    return EditMapView;

});

//var map;
//var chicago = new google.maps.LatLng(41.850033, -87.6500523);

///**
//* The HomeControl adds a control to the map that
//* returns the user to the control's defined home.
//*/

//// Define a property to hold the Home state
//HomeControl.prototype.home_ = null;

//// Define setters and getters for this property
//HomeControl.prototype.getHome = function () {
//    return this.home_;
//}

//HomeControl.prototype.setHome = function (home) {
//    this.home_ = home;
//}

//function HomeControl(controlDiv, map, home) {

//    // We set up a variable for this since we're adding
//    // event listeners later.
//    var control = this;

//    // Set the home property upon construction
//    control.home_ = home;

//    // Set CSS styles for the DIV containing the control
//    // Setting padding to 5 px will offset the control
//    // from the edge of the map
//    controlDiv.style.padding = '5px';

//    // Set CSS for the control border
//    var goHomeUI = document.createElement('div');
//    goHomeUI.style.backgroundColor = 'white';
//    goHomeUI.style.borderStyle = 'solid';
//    goHomeUI.style.borderWidth = '2px';
//    goHomeUI.style.cursor = 'pointer';
//    goHomeUI.style.textAlign = 'center';
//    goHomeUI.title = 'Click to set the map to Home';
//    controlDiv.appendChild(goHomeUI);

//    // Set CSS for the control interior
//    var goHomeText = document.createElement('div');
//    goHomeText.style.fontFamily = 'Arial,sans-serif';
//    goHomeText.style.fontSize = '12px';
//    goHomeText.style.paddingLeft = '4px';
//    goHomeText.style.paddingRight = '4px';
//    goHomeText.innerHTML = '<b>Home</b>';
//    goHomeUI.appendChild(goHomeText);

//    // Set CSS for the setHome control border
//    var setHomeUI = document.createElement('div');
//    setHomeUI.style.backgroundColor = 'white';
//    setHomeUI.style.borderStyle = 'solid';
//    setHomeUI.style.borderWidth = '2px';
//    setHomeUI.style.cursor = 'pointer';
//    setHomeUI.style.textAlign = 'center';
//    setHomeUI.title = 'Click to set Home to the current center';
//    controlDiv.appendChild(setHomeUI);

//    // Set CSS for the control interior
//    var setHomeText = document.createElement('div');
//    setHomeText.style.fontFamily = 'Arial,sans-serif';
//    setHomeText.style.fontSize = '12px';
//    setHomeText.style.paddingLeft = '4px';
//    setHomeText.style.paddingRight = '4px';
//    setHomeText.innerHTML = '<b>Set Home</b>';
//    setHomeUI.appendChild(setHomeText);

//    // Setup the click event listener for Home:
//    // simply set the map to the control's current home property.
//    google.maps.event.addDomListener(goHomeUI, 'click', function () {
//        var currentHome = control.getHome();
//        map.setCenter(currentHome);
//    });

//    // Setup the click event listener for Set Home:
//    // Set the control's home to the current Map center.
//    google.maps.event.addDomListener(setHomeUI, 'click', function () {
//        var newHome = map.getCenter();
//        control.setHome(newHome);
//    });
//}

//function initialize() {
//    var mapDiv = document.getElementById('map_canvas');
//    var mapOptions = {
//        zoom: 12,
//        center: chicago,
//        mapTypeId: google.maps.MapTypeId.ROADMAP
//    }
//    map = new google.maps.Map(mapDiv, mapOptions);

//    // Create the DIV to hold the control and
//    // call the HomeControl() constructor passing
//    // in this DIV.
//    var homeControlDiv = document.createElement('div');
//    var homeControl = new HomeControl(homeControlDiv, map, chicago);

//    homeControlDiv.index = 1;
//    map.controls[google.maps.ControlPosition.TOP_RIGHT].push(homeControlDiv);
//}