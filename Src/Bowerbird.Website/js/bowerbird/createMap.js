
// The following is for the draggable pin functionality, taken from here: http://www.wolfpil.de/v3/drag-from-outside.html
var map, iw, drag_area, actual;
var obj, xpos, ypos;
var z_index = 0;
var dummy;
var precision = true;
var latLonManualEdit = false;
var geocoder;
var markersArray = [];
var markerImg = "http://maps.gstatic.com/mapfiles/ms/icons/blue-dot.png";
var markerShadow = "http://maps.gstatic.com/mapfiles/kml/paddle/A_maps.shadow.png";

function DummyOView() {
    // Bind this to the map to access MapCanvasProjection
    this.setMap(map);
    // MapCanvasProjection is only available after draw has been called.
    this.draw = function () { };
}

DummyOView.prototype = new google.maps.OverlayView();

document.onmouseup = function () {

    // Unregister mousemove handler
    document.onmousemove = null;
    if (obj) { obj = null; }
};

function initDrag(e) {

    if (!e) var e = window.event;

    // Drag image's parent div element
    obj = e.target ? e.target.parentNode : e.srcElement.parentElement;
    if (obj.id != "location-pin") {
        if (e.cancelable) e.preventDefault();
        obj = null;
        return;
    }

    if (obj) {
        // The currently dragged object always gets the highest z-index
        z_index++;
        obj.style.zIndex = z_index.toString();

        xpos = e.clientX - obj.offsetLeft;
        ypos = e.clientY - obj.offsetTop;

        document.onmousemove = moveObj;
    }
    return false;
}

function moveObj(e) {

    if (obj && obj.id == "location-pin") {

        if (!e) var e = window.event;
        obj.style.left = e.clientX - xpos + "px";
        obj.style.top = e.clientY - ypos + "px";

        obj.onmouseup = function () {

            var gd = map.getDiv();
            var mLeft = gd.offsetLeft;
            var mTop = gd.offsetTop;

            var mWidth = gd.offsetWidth;
            var mHeight = gd.offsetHeight;

            var areaLeft = drag_area.offsetLeft;
            var areaTop = drag_area.offsetTop;

            var oWidth = obj.offsetWidth;
            var oHeight = obj.offsetHeight;

            // The object's pixel position relative to the document
            var x = obj.offsetLeft + areaLeft + oWidth / 2;
            var y = obj.offsetTop + areaTop + oHeight / 2;

            // Check if the cursor is inside the map div
            if (x > mLeft && x < (mLeft + mWidth) && y > mTop && y < (mTop + mHeight)) {

                // Difference between the x property of iconAnchor
                // and the middle of the icon width
                var anchorDiff = 1;

                // Find the object's pixel position in the map container
                var g = google.maps;
                var pixelpoint = new g.Point(x - mLeft - anchorDiff, y - mTop + (oHeight / 2));

                // Corresponding geo point on the map
                var proj = dummy.getProjection();
                var latlng = proj.fromContainerPixelToLatLng(pixelpoint);

                // Create a corresponding marker on the map
                var src = obj.firstChild.getAttribute("src");
                createDraggedMarker(latlng, src);

                // Create dragged marker anew
                fillMarker();
                //markerLatLong = latlng;
                reverseGeocode();
            }
        };
    }
    return false;
}

function fillMarker() {

    var m = document.createElement("div");
    //        m.style.position = "absolute";
    //        m.style.width = "32px";
    //        m.style.height = "32px";

    //        var left;
    //        if (obj.id == "m1") {
    //            left = "0px";
    //        } else if (obj.id == "m2") {
    //            left = "50px";
    //        } else if (obj.id == "m3") {
    //            left = "100px";
    //        }
    //        m.style.left = left;

    //        // Set the same id and class attributes again
    //        // m.setAttribute("id", obj.id);
    //        // m.setAttribute((document.all?"className":"class"), "drag");
    //        m.id = obj.id;
    //        m.className = "drag";

    //        // Append icon
    //        var img = document.createElement("img");
    //        img.src = obj.firstChild.getAttribute("src");
    //        img.style.width = "32px";
    //        img.style.height = "32px";
    //        m.appendChild(img);
    drag_area.replaceChild(m, obj);

    // Clear initial object
    obj = null;
}

function highestOrder() {

    /**
    * The currently dragged marker on the map
    * always gets the highest z-index too
    */
    return z_index;
}

function createDraggedMarker(point, src) {

    clearMarker();

    var g = google.maps;

    var image = new g.MarkerImage(src,
           new g.Size(32, 32),
           new g.Point(0, 0),
           new g.Point(15, 32)
           );

    var shadow = new g.MarkerImage(markerShadow,
           new g.Size(59, 32),
           new g.Point(0, 0),
           new g.Point(15, 32)
           );

    var marker = new g.Marker({ position: point, map: map,
        clickable: true,
        draggable: true,
        raiseOnDrag: false,
        icon: image, shadow: shadow, zIndex: highestOrder()
    });

    markersArray.push(marker);

    g.event.addListener(marker, "drag", function () {
        actual = marker;
        var lat = actual.getPosition().lat();
        var lng = actual.getPosition().lng();

        //            iw.setContent(lat.toFixed(6) + ", " + lng.toFixed(6));
        //            iw.open(map, this);
        $("#latitude").val(lat);
        $("#longitude").val(lng);
    });

    g.event.addListener(marker, "dragstart", function () {
        // Close infowindow when dragging the marker whose infowindow is open
        if (actual == marker) iw.close();
        // Increment z_index
        z_index++;
        marker.setZIndex(highestOrder());
    });

    g.event.addListener(marker, "dragend", function () {
        displayLatLong();
        reverseGeocode();
    });

    displayLatLong();
}

function buildMap() {
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

    map = new g.Map(document.getElementById("location-map"), mapSettings);
    iw = new g.InfoWindow();
    geocoder = new g.Geocoder();

    // v2 behaviour
    g.event.addListener(map, "click", function () {
        if (iw) iw.close();
    });

    // Register mousedown listener
    drag_area = document.getElementById("pin-field");
    drag_area.onmousedown = initDrag;

    // Add a dummy overlay for later use.
    // Needed for API v3 to convert pixels to latlng.
    dummy = new DummyOView();

    manualLatLonEdit(false);

    //http://tech.cibul.net/geocode-with-google-maps-api-v3/
    $(function () {
        $("#address").autosearch({
            //This bit uses the geocoder to fetch address values
            source: function (request, response) {
                geocoder.geocode({ 'address': request.term }, function (results, status) {
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
                event.preventDefault();
                var location = new g.LatLng(ui.item.latitude, ui.item.longitude);
                createDraggedMarker(location, markerImg);
                fillMarker();
                reverseGeocode();
            }
        });
    });

    $('#anonymiselocation').click(function () {

        if ($(this).is(':checked')) {
            precision = false;
        }
        else {
            precision = true;
        }

        displayLatLong();
    });

    $('#latitude').bind('focus', function () {
        manualLatLonEdit(true);
    });

    $('#longitude').bind('focus', function () {
        manualLatLonEdit(true);
    });

    $('#latitude').bind('blur', function () {
        manualLatLonEdit(false);
    });

    $('#longitude').bind('blur', function () {
        manualLatLonEdit(false);
    });

    $('#latitude').bind('change', function () {
        if (latLonManualEdit) {
            checkLocation();
        }
    });

    $('#longitude').bind('change', function () {
        if (latLonManualEdit) {
            checkLocation();
        }
    });

    $('#find-latlon-button').bind('click', function () {
        checkLocation();
        manualLatLonEdit(false);
    });

}

//----------------map functions imported from PaDIL-------------------------------

function clearMarker() {
    if (markersArray) {
        for (i in markersArray) {
            markersArray[i].setMap(null);
        }
        markersArray = new Array();
        markerLatLong = null;
    }
}

function manualLatLonEdit(isManual) {
    latLonManualEdit = isManual;
    if (latLonManualEdit) {
        $('#find-latlon-button').show();
    }
    else {
        $('#find-latlon-button').hide();
    }
}

function reverseGeocode() {
    if (markersArray[0]) {
        var marker = markersArray[0];
        reverseGeocodedLast = new Date();
        geocoder.geocode({ latLng: marker.getPosition() }, reverseGeocodeResult);
    }
}

function reverseGeocodeResult(results, status) {
    currentReverseGeocodeResponse = results;
    if (status == 'OK') {
        if (results.length == 0) {
            $('#address').val('');
        } else {
            var addressResult = results[0].formatted_address;
            $('#address').val(addressResult);
        }
    } else {
        $('#address').val('....');
    }
}

function geocode() {
    var address = document.getElementById("address").value;
    geocoder.geocode({
        'address': address,
        'partialmatch': true
    }, geocodeResult);
}

function geocodeResult(results, status) {
    if (status == 'OK' && results.length > 0) {
        map.fitBounds(results[0].geometry.viewport);
    } else {
        alert("Geocode was not successful for the following reason: " + status);
    }
}

function displayLatLong() {

    if (markersArray[0]) {
        var marker = markersArray[0];
        var lat = marker.getPosition().lat();
        var lng = marker.getPosition().lng();

        if (precision == true) {
            $('#latitude').val(lat);
            $('#longitude').val(lng);
        }
        else {
            $('#latitude').val(parseFloat(lat).toFixed(1));
            $('#longitude').val(parseFloat(lng).toFixed(1));
        }
    }
    else {
        return;
    }
}

function checkLocation() {
    var lat = $('#latitude').val();
    var lon = $('#longitude').val();

    if (isNumber(lat) && isNumber(lon) && (lat >= -90 && lat <= 90) && (lon >= -180 && lon <= 180)) {
        markerLatLong = new google.maps.LatLng(lat, lon);
        createDraggedMarker(markerLatLong, markerImg);
        reverseGeocode();
    }
}

function isNumber(n) {
    return !isNaN(parseFloat(n)) && isFinite(n);
}

//----------------end map functions imported from PaDIL-------------------------------