// ====================================================================================
// Namespaces
// ====================================================================================

window.Bowerbird = window.Bowerbird || {

    Models: {},

    Collections: {},

    Views: {}

};

// ====================================================================================
// Models
// ====================================================================================

// User
window.Bowerbird.Models.User = Backbone.Model.extend({

    defaults: {
        id: ''
    }

});

// Team
window.Bowerbird.Models.Team = Backbone.Model.extend({

    defaults: {
        id: '',
        name: '',
        description: ''
    }

});

// Project
window.Bowerbird.Models.Project = Backbone.Model.extend({

    defaults: {
        id: '',
        name: '',
        description: ''
    }

});

// Watch
window.Bowerbird.Models.Watch = Backbone.Model.extend({

    defaults: {
        id: '',
        name: '',
        description: ''
    }

});

// Observation
window.Bowerbird.Models.Observation = Backbone.Model.extend({

    defaults: {
        title: '',
        latitude: null,
        longitude: null,
        address: '',
        observedOn: null,
        isIdentificationRequired: false,
        observationCategory: '',
        mediaResources: []
    }

});

// Stream
window.Bowerbird.Models.Stream = Backbone.Model.extend({

//    defaults: {
//        type: null,
//        groupId: null,
//        watchListId: null,
//        key: null,
//        filter: null,
//        context: null
//    },

    initialize: function (attributes) {
        
    },

    toJSON: function () {
        var json = _.clone(this.attributes);
        return _.extend(json, { context: this.get("context").toJSON() });
    },

    loadStreamItems: function () {
        //var ids = this.get("invitees") || [];

        this.streamItems = new Bowerbird.Collections.StreamItems();
        this.streamItems.fetch();
    }

});

// Event
window.Bowerbird.Models.Event = Backbone.Model.extend({

    defaults: {
        type: null,
        payload: null,
        timestamp: null
    }

});

// StreamItem
window.Bowerbird.Models.StreamItem = Backbone.Model.extend({

    defaults: {
        type: null
    }

});

// ====================================================================================
// Collections
// ====================================================================================

// Paginated Collection
window.Bowerbird.Collections.PaginatedCollection = Backbone.Collection.extend({

    initialize: function () {
        _.bindAll(this, 'parse', 'url', 'pageInfo', 'nextPage', 'previousPage');
        typeof (options) != 'undefined' || (options = {});
        this.page = 1;
        typeof (this.pageSize) != 'undefined' || (this.pageSize = 10);
    },
    fetch: function (options) {
        typeof (options) != 'undefined' || (options = {});
        this.trigger("fetching");
        var self = this;
        var success = options.success;
        options.success = function (resp) {
            self.trigger("fetched");
            if (success) { success(self, resp); }
        };
        return Backbone.Collection.prototype.fetch.call(this, options);
    },
    parse: function (resp) {
        this.page = resp.Page;
        this.pageSize = resp.PageSize;
        this.total = resp.TotalResultCount;
        return resp.PagedListItems;
    },
    url: function () {
        return this.baseUrl + '?' + $.param({ page: this.page, pageSize: this.pageSize });
    },
    pageInfo: function () {
        var info = {
            total: this.total,
            page: this.page,
            pageSize: this.pageSize,
            pages: Math.ceil(this.total / this.pageSize),
            prev: false,
            next: false
        };

        var max = Math.min(this.total, this.page * this.pageSize);

        if (this.total == this.pages * this.pageSize) {
            max = this.total;
        }

        info.range = [(this.page - 1) * this.pageSize + 1, max];

        if (this.page > 1) {
            info.prev = this.page - 1;
        }

        if (this.page < info.pages) {
            info.next = this.page + 1;
        }

        return info;
    },
    nextPage: function () {
        if (!this.pageInfo().next) {
            return false;
        }
        this.page = this.page + 1;
        return this.fetch();
    },
    previousPage: function () {
        if (!this.pageInfo().prev) {
            return false;
        }
        this.page = this.page - 1;
        return this.fetch();
    }

});

// StreamItems
window.Bowerbird.Collections.StreamItems = Bowerbird.Collections.PaginatedCollection.extend({

    model: Bowerbird.Models.StreamItem,

//    initialize: function (options) {
//        this.pageSize = 10;
//        this.page = 1;
//    },

    baseUrl: "/members/streamitem/list"

});

// Observations
window.Bowerbird.Collections.Observations = Backbone.Collection.extend({

    model: Bowerbird.Models.Observation,

    url: "/members/observation"

});

// Events
window.Bowerbird.Collections.Events = Backbone.Collection.extend({

    model: Bowerbird.Models.Event,

    initialize: function (options) {
        this.context = options.context;
    },

    url: function () {
        return "/observation/events?context=" + this.context;
    }

});

// ====================================================================================
// Views
// ====================================================================================

// Header
window.Bowerbird.Views.HeaderView = Backbone.View.extend({

    el: 'header',

    initialize: function () {
    }

});

// Navigation
window.Bowerbird.Views.SidebarView = Backbone.View.extend({

    el: '#sidebar',

    initialize: function (options) {
        _.extend(this, Backbone.Events);
    }

});

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

        if (isNumber(lat) && isNumber(lon) && (lat >= -90 && lat <= 90) && (lon >= -180 && lon <= 180))
        {
            markerLatLong = new google.maps.LatLng(lat, lon);
            createDraggedMarker(markerLatLong, markerImg);
            reverseGeocode();
        }
    }

    function isNumber(n) {
        return !isNaN(parseFloat(n)) && isFinite(n);
    }

//----------------end map functions imported from PaDIL-------------------------------


//    var uploader;
//    var mediaResources = new Array();

//    function buildUploader(recordType) {
//        uploader = new qq.FileUploader({
//            element: document.getElementById('media-uploader'),
//            action: '/members/mediaresource/'+recordType+'upload',

//            template: '<div class="media-resource-items" id="media-resource-items"><ul class="qq-upload-list" style="display: none;"></ul><div class="media-resource-dropzone">Drop Files Here</div></div><div class="field"><div>' +
//                    '<div id="media-resource-upload-button" class="button media-resource-upload-button">Choose Files</div>' +
//                    '<input type="button" value="Import From Website" id="media-resource-import-button"/></div></div>',
//            fileTemplate: '<li>' +
//                    '<span class="qq-upload-file"></span>' +
//                    '<span class="qq-upload-spinner"></span>' +
//                    '<span class="qq-upload-size"></span>' +
//                    '<a class="qq-upload-cancel" href="#">Cancel</a>' +
//                '</li>',

//            classes: {
//                // used to get elements from templates
//                button: 'media-resource-upload-button', //'qq-upload-button',
//                drop: 'media-resource-dropzone', //'qq-upload-drop-area',
//                dropActive: 'qq-upload-drop-area-active',
//                list: 'qq-upload-list',

//                file: 'qq-upload-file',
//                spinner: 'qq-upload-spinner',
//                size: 'qq-upload-size',
//                cancel: 'qq-upload-cancel',

//                // added to list item when upload completes
//                // used in css to hide progress spinner
//                success: 'qq-upload-success',
//                fail: 'qq-upload-fail'
//                },

//            multiple: true,
//            debug: true,
//            onComplete: function (id, fileName, responseText) {
//                $('#media-resource-items').append('<div class="media-resource-uploaded"><img src="' + responseText.imageUrl + '" width="200px" /><div><span>' + responseText.fileName + '</span><span>' + responseText.fileSize + '<span></div></div>');
//                mediaResources.push(responseText.Id);
//            }
//        });
//    }

// Workspace
    window.Bowerbird.Views.WorkspaceView = Backbone.View.extend({

        el: 'article',

        initialize: function (options) {
            _.extend(this, Backbone.Events);
            this.workspaceSubViews = [];
        },

        showStream: function (type, key, filter) {
            $('.form-workspace-item').remove();
            var context = null;

            switch (type) {
                case "user":
                    context = new Bowerbird.Models.User();
                    break;
                case "team":
                    context = new Bowerbird.Models.Team();
                    break;
                case "project":
                    context = new Bowerbird.Models.Project();
                    break;
                case "watch":
                    context = new Bowerbird.Models.Watch();
                    break;
            }

            var stream = new Bowerbird.Models.Stream({ type: type, filter: filter, key: key, context: context });
            var streamView = new Bowerbird.Views.StreamView({ id: "stream-" + type + "-" + key, model: stream });

            this.workspaceSubViews.push(streamView);

            streamView.render();
            //streamView.loadStreamItems();
            //streamWorkspace.bind("newObservation", this.showObservationForm, this);
            //this.showWorkspaceItem(streamWorkspaceItem);
            //$('.stream-workspace-item').show();
        },

        showObservationForm: function () {
            $('.stream-workspace-item').hide();

            var observation = new Bowerbird.Models.Observation();
            var formWorkspaceItem = new Bowerbird.Views.FormWorkspaceItem({ id: 'form-create-observation', model: observation });
            this.showWorkspaceItem(formWorkspaceItem);

            //        var mapSettings = {
            //            center: new google.maps.LatLng(-29.191427, 134.472126), // Centre on Australia
            //            zoom: 4,
            //            panControl: false,
            //            streetViewControl: false,
            //            mapTypeControl: false,
            //            mapTypeId: google.maps.MapTypeId.TERRAIN
            //        };

            //        var map = new google.maps.Map(document.getElementById("location-map"), mapSettings);
            $("#observedOn").datepicker({ dateFormat: "d MM yy", changeMonth: true, changeYear: true, showOn: "both", buttonText: "..." });
            //            $("").click(function() {
            //                
            //            });
            buildMap();
            //buildUploader();
            buildMediaUploader('observation', true);
        },

        showWorkspaceItem: function (workspaceItem) {
            // Hide current workspace if it is a stream
            if (workspaceItem instanceof Bowerbird.Views.StreamWorkspaceItem) {
                while (this.workspaceSubViews.length > 0) {
                    var currentWorkspaceItem = this.workspaceSubViews.shift();
                    currentWorkspaceItem.hide($(currentWorkspaceItem.el));
                }
            }

            // Show new workspace
            this.workspaceSubViews.unshift(workspaceItem);
            workspaceItem.render();
            workspaceItem.loadEvents();
        }

    });

//// Workspace Item
//window.Bowerbird.Views.WorkspaceItem = Backbone.View.extend({

//    renderWorkspaceItem: function (workspaceElement, html) {
//        workspaceElement.html(html);
//        workspaceElement.appendTo("#stream");

//        workspaceElement
//            .css({ zIndex: 98 })
//            .animate({
//                left: parseInt(workspaceElement.css('left'), 10) == 0 ? -workspaceElement.outerWidth() : 0
//            },
//            500);
//    },

//    hide: function (workspaceElement) {
//        
//        /*var that = this;
//        workspaceElement
//        .css({ zIndex: 98 })
//        .animate({
//        left: parseInt(workspaceElement.css('left'), 10) == 0 ? -workspaceElement.outerWidth() : 0
//        //opacity: 0
//        },
//        300,
//        function () {
//        that.remove();
//        });*/
//    }

//});

// Stream Workspace Item
    window.Bowerbird.Views.StreamView = Backbone.View.extend({

        id: 'stream',

        //className: 'stream-workspace-item',

        events: {
            "click #create-observation": "newObservation"
        },

        initialize: function (options) {
            _.extend(this, Backbone.Events);

            this.streamItems = new Bowerbird.Collections.StreamItems({ context: "butterflies-of-aus" });

            this.streamItems.bind("add", this.eventItemsChanged, this);
            this.streamItems.bind("reset", this.eventItemsReset);
            alert("hells yeah");
            //this.streamItems.fetch();
            this.loadStreamItems();
        },

        render: function () {
            //var compiledHtml = $.tmpl($("#observation-stream-item-template"), this.model.toJSON());
            //this.renderWorkspaceItem($(this.el), compiledHtml);
            //this.render($(this.el), compiledHtml);
            return this;
        },

        newObservation: function () {
            this.trigger('newObservation', this.model);
        },

        eventItemsReset: function () {
            console.log("changed.");
        },

        eventItemsFetchSuccess: function () {
            console.log("success.");
        },

        eventItemsFetchError: function () {
            console.log("failed.");
        },

        loadStreamItems: function () {
            console.log("loading stream items...");
            this.streamItems.fetch({
                data: {
                    userid: 'users/68'
                },
                success: this.eventItemsFetchSuccess,
                error: this.eventItemsFetchError
            });
        }

    });

//// Form Workspace Item
//window.Bowerbird.Views.FormWorkspaceItem = Bowerbird.Views.WorkspaceItem.extend({

//    className: 'form workspace-item form-workspace-item',

//    events: {
//        "click #cancel": "cancel",
//        "click #save": "save"
//    },

//    initialize: function (options) {
//        _.extend(this, Backbone.Events);
//    },

//    render: function () {
//        var compiledHtml = $.tmpl($("#workspace-form-create-observation-template"), this.model.toJSON());
//        this.renderWorkspaceItem($(this.el), compiledHtml);
//        return this;
//    },

//    cancel: function () {
//        //this.hide($(this.el));
//        $('.stream-workspace-item').show();
//        $('.form-workspace-item').remove();
//    },

//    save: function () {
//        this.model.set({
//            "title": $("#title").attr("value"),
//            "address": $("#address").attr("value"),
//            "latitude": $("#latitude").attr("value"),
//            "longitude": $("#longitude").attr("value"),
//            "observedOn": $("#observedOn").attr("value"),
//            "isIdentificationRequired": $("#isIdentificationRequired").attr("value"),
//            "observationCategory": $("#observationCategory").attr("value")
//        });

//        //window.observations.add(this.model);

//        this.model.save();

//        this.hide($(this.el));
//    },

//    loadEvents: function () {
//        console.log("loading events...");
//    }

//});

window.observations = new Bowerbird.Collections.Observations();

// ====================================================================================
// Routers
// ====================================================================================

// AppRouter
window.Bowerbird.AppRouter = Backbone.Router.extend({

    routes: {
        "": "showUserStream",
        "/user/stream/:filter": "showUserStream",
        "/observation/create": "showObservationCreate"
        //"/team/:key/stream/:filter": "showTeamStream",
        //"/project/:key/stream/:filter": "showProjectStream",
        //"/watch/:key/stream/:filter": "showWatchStream"
        //"search/:query/p:page": "search"   // #search/kiwis/p7
    },

    initialize: function (options) {
        this.headerView = new Bowerbird.Views.HeaderView({ app: this });
        //this.sidebarView = new Bowerbird.Views.SidebarView({ app: this });
        this.workspaceView = new Bowerbird.Views.WorkspaceView({ app: this });
    },

    showObservationCreate: function () {
        //this.workspaceView.showObservationForm();
    },

    showUserStream: function (filter) {
        this.workspaceView.showStream("user", "user", filter);
    }

});