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

    defaults: {
        type: null,
        key: null,
        filter: null,
        context: null,
        streamItems: []
    },

    toJSON: function () {
        var json = _.clone(this.attributes);
        return _.extend(json, { context: this.get("context").toJSON() });
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

// ====================================================================================
// Collections
// ====================================================================================

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
window.Bowerbird.Views.Header = Backbone.View.extend({

    el: '#header',

    initialize: function () {
    }

});

// Navigation
window.Bowerbird.Views.Navigation = Backbone.View.extend({

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

        var g = google.maps;
        var image = new g.MarkerImage(src,
   new g.Size(32, 32),
   new g.Point(0, 0),
   new g.Point(15, 32));

        var shadow = new g.MarkerImage("http://maps.gstatic.com/mapfiles/kml/paddle/A_maps.shadow.png",
   new g.Size(59, 32),
   new g.Point(0, 0),
   new g.Point(15, 32));

        var marker = new g.Marker({ position: point, map: map,
            clickable: true, 
            draggable: true,
            raiseOnDrag: false,
            icon: image, shadow: shadow, zIndex: highestOrder()
        });

//        g.event.addListener(marker, "click", function () {
//            actual = marker;
//            var lat = actual.getPosition().lat();
//            var lng = actual.getPosition().lng();

//            //            iw.setContent(lat.toFixed(6) + ", " + lng.toFixed(6));
//            //            iw.open(map, this);
//            $("#latitude").val(lat);
//            $("#longitude").val(lng);
//        });

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

        var lat = marker.getPosition().lat();
        var lng = marker.getPosition().lng();
        $("#latitude").val(lat);
        $("#longitude").val(lng);
    }


    function buildMap() {
        var g = google.maps;
        var mapSettings = {
            center: new g.LatLng(-29.191427, 134.472126), // Centre on Australia
            zoom: 4,
            panControl: false,
            streetViewControl: false,
            mapTypeControl: true,
            mapTypeId: g.MapTypeId.TERRAIN
        };

        map = new g.Map(document.getElementById("location-map"), mapSettings);
        iw = new g.InfoWindow();

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
    }

// Workspace
    window.Bowerbird.Views.Workspace = Backbone.View.extend({

        el: '#workspace',

        initialize: function (options) {
            _.extend(this, Backbone.Events);
            this.workspaceItems = [];
        },

        showStream: function (type, key, filter) {
            $('.form-workspace-item').remove();
            var streamContext = null;

            switch (type) {
                case "user":
                    streamContext = new Bowerbird.Models.User();
                    break;
                case "team":
                    streamContext = new Bowerbird.Models.Team();
                    break;
                case "project":
                    streamContext = new Bowerbird.Models.Project();
                    break;
                case "watch":
                    streamContext = new Bowerbird.Models.Watch();
                    break;
            }

            var stream = new Bowerbird.Models.Stream({ type: type, filter: filter, key: key, context: streamContext });
            var streamWorkspaceItem = new Bowerbird.Views.StreamWorkspaceItem({ id: "stream-" + type + "-" + key, model: stream });

            //streamWorkspace.bind("newObservation", this.showObservationForm, this);

            //this.showWorkspaceItem(streamWorkspaceItem);
            $('.stream-workspace-item').show();
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
            $("").click(function() {
                
            });
            buildMap();
        },

        showWorkspaceItem: function (workspaceItem) {
            // Hide current workspace if it is a stream
            if (workspaceItem instanceof Bowerbird.Views.StreamWorkspaceItem) {
                while (this.workspaceItems.length > 0) {
                    var currentWorkspaceItem = this.workspaceItems.shift();
                    currentWorkspaceItem.hide($(currentWorkspaceItem.el));
                }
            }

            // Show new workspace
            this.workspaceItems.unshift(workspaceItem);
            workspaceItem.render();
            workspaceItem.loadEvents();
        }

    });

// Workspace Item
window.Bowerbird.Views.WorkspaceItem = Backbone.View.extend({

    renderWorkspaceItem: function (workspaceElement, html) {
        workspaceElement.html(html);
        workspaceElement.appendTo("#large-workspace");

        workspaceElement
            .css({ zIndex: 98 })
            .animate({
                left: parseInt(workspaceElement.css('left'), 10) == 0 ? -workspaceElement.outerWidth() : 0
            },
            500);
    },

    hide: function (workspaceElement) {
        
        /*var that = this;
        workspaceElement
        .css({ zIndex: 98 })
        .animate({
        left: parseInt(workspaceElement.css('left'), 10) == 0 ? -workspaceElement.outerWidth() : 0
        //opacity: 0
        },
        300,
        function () {
        that.remove();
        });*/
    }

});

// Stream Workspace Item
window.Bowerbird.Views.StreamWorkspaceItem = Bowerbird.Views.WorkspaceItem.extend({

    className: 'workspace-item stream-workspace-item',

    events: {
        "click #create-observation": "newObservation"
    },

    initialize: function (options) {
        _.extend(this, Backbone.Events);

        this.eventItems = new Bowerbird.Collections.Events({ context: "butterflies-of-aus" });

        this.eventItems.bind("add", this.eventItemsChanged, this);
        this.eventItems.bind("reset", this.eventItemsReset);

        //this.eventItems.fetch();
    },

    render: function () {
        var compiledHtml = $.tmpl($("#workspace-stream-template"), this.model.toJSON());
        this.renderWorkspaceItem($(this.el), compiledHtml);
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

    loadEvents: function () {
        console.log("loading events...");
        this.eventItems.fetch({ success: this.eventItemsFetchSuccess, error: this.eventItemsFetchError });
    }

});

// Form Workspace Item
window.Bowerbird.Views.FormWorkspaceItem = Bowerbird.Views.WorkspaceItem.extend({

    className: 'form workspace-item form-workspace-item',

    events: {
        "click #cancel": "cancel",
        "click #save": "save"
    },

    initialize: function (options) {
        _.extend(this, Backbone.Events);
    },

    render: function () {
        var compiledHtml = $.tmpl($("#workspace-form-create-observation-template"), this.model.toJSON());
        this.renderWorkspaceItem($(this.el), compiledHtml);
        return this;
    },

    cancel: function () {
        //this.hide($(this.el));
        $('.stream-workspace-item').show();
        $('.form-workspace-item').remove();
    },

    save: function () {
        this.model.set({
            "title": $("#title").attr("value"),
            "address": $("#address").attr("value"),
            "latitude": $("#latitude").attr("value"),
            "longitude": $("#longitude").attr("value"),
            "observedOn": $("#observedOn").attr("value"),
            "isIdentificationRequired": $("#isIdentificationRequired").attr("value"),
            "observationCategory": $("#observationCategory").attr("value")
        });

        //window.observations.add(this.model);

        this.model.save();

        this.hide($(this.el));
    },

    loadEvents: function () {
        console.log("loading events...");
    }

});

window.observations = new Bowerbird.Collections.Observations();

// ====================================================================================
// Routers
// ====================================================================================

// AppRouter
window.Bowerbird.AppRouter = Backbone.Router.extend({

    routes: {
        "/user/stream/:filter": "showUserStream",
        "/observation/create": "showObservationCreate"
        //"/team/:key/stream/:filter": "showTeamStream",
        //"/project/:key/stream/:filter": "showProjectStream",
        //"/watch/:key/stream/:filter": "showWatchStream"
        //"search/:query/p:page": "search"   // #search/kiwis/p7
    },

    initialize: function (options) {
        this.header = new Bowerbird.Views.Header({ app: this });
        //this.navigation = new Bowerbird.Views.Navigation({ app: this });
        this.workspace = new Bowerbird.Views.Workspace({ app: this });
    },

    showObservationCreate: function () {
        this.workspace.showObservationForm();
    },

    showUserStream: function (filter) {
        this.workspace.showStream("user", "user", filter);
    }

    //    showTeamStream: function (key, filter) {
    //        this.workspaceContainer.showStream("team", key, filter);
    //    },

    //    showProjectStream: function (key, filter) {
    //        this.workspaceContainer.showStream("project", key, filter);
    //    },

    //    showWatchStream: function (key, filter) {
    //        this.workspaceContainer.showStream("watch", key, filter);
    //    }

    //    search: function (query, page) {

    //    }

});
