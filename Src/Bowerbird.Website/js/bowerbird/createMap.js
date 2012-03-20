
//// The following is for the draggable pin functionality, taken from here: http://www.wolfpil.de/v3/drag-from-outside.html
//var map, iw, drag_area, actual;
//var obj, xpos, ypos;
//var z_index = 0;
////var dummy;
//var precision = true;
//var geocoder;
//var mapMarker = null;
//var markerImg = "http://maps.gstatic.com/mapfiles/ms/icons/blue-dot.png";
//var markerShadow = "http://maps.gstatic.com/mapfiles/kml/paddle/A_maps.shadow.png";

//function DummyOverlayView(map) {
//    // Bind this to the map to access MapCanvasProjection
//    this.setMap(map);
//    // MapCanvasProjection is only available after draw has been called.
//    this.draw = function () { };
//}

//DummyOverlayView.prototype = new google.maps.OverlayView();

//function removeMarkerFromPlaceholder() {
//    $('#location-pin').remove();
//}

 

//function positionMarker(point, src) {

//    // Remove map marker first
//    if (mapMarker) {
//        mapMarker.setMap(null);
//        mapMarker = null;
//    }

//    var g = google.maps;

//    var image = new g.MarkerImage(src,
//           new g.Size(32, 32),
//           new g.Point(0, 0),
//           new g.Point(15, 32)
//           );

//    var shadow = new g.MarkerImage(markerShadow,
//           new g.Size(59, 32),
//           new g.Point(0, 0),
//           new g.Point(15, 32)
//           );

//    mapMarker = new g.Marker({ position: point, map: map,
//        clickable: true,
//        draggable: true,
//        raiseOnDrag: false,
//        icon: image, shadow: shadow, zIndex: highestOrder()
//    });

//    g.event.addListener(mapMarker, "drag", function () {
////        var lat = mapMarker.getPosition().lat();
////        var lng = mapMarker.getPosition().lng();
////        iw.setContent(lat.toFixed(6) + ", " + lng.toFixed(6));
////        iw.open(map, this);
//        displayLatLong();
//    });

//    g.event.addListener(mapMarker, "dragstart", function () {
//        // Close infowindow when dragging the marker whose infowindow is open
//        if (actual == mapMarker) iw.close();
//        // Increment z_index
//        z_index++;
//        mapMarker.setZIndex(highestOrder());
//    });

//    g.event.addListener(mapMarker, "dragend", function () {
//        displayLatLong();
//        reverseGeocode();
//    });

//    displayLatLong();
//}

//function buildMap() {
//    var g = google.maps;

//    var mapSettings = {
//        center: new g.LatLng(-29.191427, 134.472126), // Centre on Australia
//        zoom: 4,
//        panControl: false,
//        streetViewControl: false,
//        mapTypeControl: true,
//        scrollwheel: false,
//        mapTypeId: g.MapTypeId.TERRAIN
//    };

//    map = new g.Map(document.getElementById("location-map"), mapSettings);
//    iw = new g.InfoWindow();
//    geocoder = new g.Geocoder();

//    // Add a dummy overlay for later use.
//    // Needed for API v3 to convert pixels to latlng.
//    dummy = new DummyOverlayView();

//    //http://tech.cibul.net/geocode-with-google-maps-api-v3/
//    $(function () {
//        $("#address").autocomplete({
//            //This bit uses the geocoder to fetch address values
//            source: function (request, response) {
//                geocoder.geocode({ 'address': request.term, 'region': 'AU' }, function (results, status) {
//                    response($.map(results, function (item) {
//                        return {
//                            label: item.formatted_address,
//                            value: item.formatted_address,
//                            latitude: item.geometry.location.lat(),
//                            longitude: item.geometry.location.lng()
//                        }
//                    }));
//                })
//            },
//            //This bit is executed upon selection of an address
//            select: function (event, ui) {
//                var location = new g.LatLng(ui.item.latitude, ui.item.longitude);
//                positionMarker(location, markerImg);
//                removeMarkerFromPlaceholder();
//                reverseGeocode();
//            }
//        });
//    });


//    $('#anonymiselocation').click(function () {
//        if ($(this).is(':checked')) {
//            precision = false;
//        }
//        else {
//            precision = true;
//        }
//        displayLatLong();
//    });

//}

//----------------map functions imported from PaDIL-------------------------------

//function reverseGeocode() {
//    if (mapMarker) {
//        reverseGeocodedLast = new Date();
//        geocoder.geocode({ latLng: mapMarker.getPosition() }, reverseGeocodeResult);
//    }
//}

//function reverseGeocodeResult(results, status) {
//    currentReverseGeocodeResponse = results;
//    if (status == 'OK') {
//        if (results.length == 0) {
//            $('#address').val('');
//        } else {
//            var addressResult = results[0].formatted_address;
//            $('#address').val(addressResult);
//        }
//    } else {
//        $('#address').val('');
//    }
//    $('#address').change();
//}

//function geocode() {
//    var address = document.getElementById("address").value;
//    geocoder.geocode({
//        'address': address,
//        'partialmatch': true
//    }, geocodeResult);
//}

//function geocodeResult(results, status) {
//    if (status == 'OK' && results.length > 0) {
//        map.fitBounds(results[0].geometry.viewport);
//    } else {
//        alert("Geocode was not successful for the following reason: " + status);
//    }
//}

//function displayLatLong() {
//    if(mapMarker) {
//        var lat = mapMarker.getPosition().lat();
//        var lng = mapMarker.getPosition().lng();
//        if (precision == true) {
//            $('#latitude').val(lat);
//            $('#longitude').val(lng);
//        }
//        else {
//            $('#latitude').val(parseFloat(lat).toFixed(1));
//            $('#longitude').val(parseFloat(lng).toFixed(1));
//        }
//        $('#latitude').change();
//        $('#longitude').change();
//    }
//    else {
//        return;
//    }
//}

//function checkLocation() {
//    var lat = $('#latitude').val();
//    var lon = $('#longitude').val();

//    if (isNumber(lat) && isNumber(lon) && (lat >= -90 && lat <= 90) && (lon >= -180 && lon <= 180)) {
//        var markerLatLong = new google.maps.LatLng(lat, lon);
//        positionMarker(markerLatLong, markerImg);
//        reverseGeocode();
//    }
//}

//function isNumber(n) {
//    return !isNaN(parseFloat(n)) && isFinite(n);
//}

//----------------end map functions imported from PaDIL-------------------------------