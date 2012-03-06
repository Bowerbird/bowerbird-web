/*************************************************************
Namespace
*************************************************************/

window.Bowerbird = window.Bowerbird || {
    Models: {},
    Collections: {},
    Views: {}
};

/*************************************************************
Models
*************************************************************/

window.Bowerbird.Models.Stream = Backbone.Model.extend({
    defaults: {
        context: null,
        filter: null,
        uri: ''
    },

    initialize: function (options) {
        _.extend(this, Backbone.Events);
        _.bindAll(this);
        this.streamItems = new Bowerbird.Collections.StreamItems();
    },

    setNewStream: function (context, filter) {
        this.set('context', context);
        this.set('filter', filter);
        var uri = '';
        if (this.has('context')) {
            uri = this.get('context').get('id');
        } else {
            uri = 'home';
        }
        uri += '/' + this.get('filter');
        this.set('uri', uri);
        this.trigger('newStream', this);
        this.trigger('fetchingItemsStarted', this);
        this.streamItems.fetchFirstPage(this);
    },

    setNewFilter: function (filter) {
        this.set('filter', filter);
        this.trigger('newStreamFilter', this);
        this.trigger('fetchingItemsStarted', this);
        this.streamItems.fetchFirstPage(this);
    },

    setNextPage: function () {
        this.trigger('newStreamPage', this);
        this.trigger('fetchingItemsStarted', this);
        this.streamItems.fetchNextPage(this);
    }
});

window.Bowerbird.Models.StreamItem = Backbone.Model.extend({
});

window.Bowerbird.Models.Team = Backbone.Model.extend({
});

window.Bowerbird.Models.Project = Backbone.Model.extend({
});

window.Bowerbird.Models.User = Backbone.Model.extend({
});

window.Bowerbird.Models.Observation = Backbone.Model.extend({
});

/*************************************************************
Collections
*************************************************************/

window.Bowerbird.Collections.PaginatedCollection = Backbone.Collection.extend({
    initialize: function () {
        _.extend(this, Backbone.Events);
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

    _firstPage: function (options) {
        this.page = 1;
        return this.fetch(options);
    },

    _nextPage: function (options) {
        if (!this.pageInfo().next) {
            return false;
        }
        this.page = this.page + 1;
        return this.fetch(options);
    },

    _previousPage: function () {
        if (!this.pageInfo().prev) {
            return false;
        }
        this.page = this.page - 1;
        return this.fetch(options);
    }
});

window.Bowerbird.Collections.StreamItems = Bowerbird.Collections.PaginatedCollection.extend({
    model: Bowerbird.Models.StreamItem,

    baseUrl: '/members/streamitem/list',

    initialize: function () {
        _.extend(this, Backbone.Events);
        _.bindAll(this, '_onSuccess', '_onSuccessWithAddFix', '_getFetchOptions');
        Bowerbird.Collections.PaginatedCollection.prototype.initialize.apply(this, arguments);
    },

    fetchFirstPage: function (stream) {
        this._firstPage(this._getFetchOptions(stream, false));
    },

    fetchNextPage: function (stream) {
        this._nextPage(this._getFetchOptions(stream, true));
    },

    _getFetchOptions: function (stream, add) {
        var options = {
            data: {},
            add: add,
            success: null
        };
        if (add) {
            options.success = this._onSuccess;
        } else {
            options.success = this._onSuccessWithAddFix;
        }
        if (stream.get('context') != null) {
            if (stream.get('context') instanceof Bowerbird.Models.Team || stream.get('context') instanceof Bowerbird.Models.Project) {
                options.data.groupId = stream.get('context').get('id');
            } else if (stream.get('context') instanceof Bowerbird.Models.User) {
                options.data.userId = stream.get('context').get('id');
            }
        }
        if (stream.get('filter') != null) {
            options.data.filter = stream.get('filter');
        }
        return options;
    },

    _onSuccess: function (collection, response) {
        app.stream.trigger('fetchingItemsComplete', app.stream, response);
    },

    _onSuccessWithAddFix: function (collection, response) {
        this._onSuccess(collection, response);
        // Added the following manual triggering of 'add' event due to Backbone bug: https://github.com/documentcloud/backbone/issues/479
        self = this;
        response.forEach(function (item) {
            self.trigger('add', item);
        });
    }
});

window.Bowerbird.Collections.Teams = Backbone.Collection.extend({
    model: Bowerbird.Models.Team,

    url: '/teams/list',

    initialize: function () {
        _.extend(this, Backbone.Events);
    }
});

window.Bowerbird.Collections.Projects = Backbone.Collection.extend({
    model: Bowerbird.Models.Project,

    url: '/projects/list',

    initialize: function () {
        _.extend(this, Backbone.Events);
    }
});

/*************************************************************
Views
*************************************************************/

window.Bowerbird.Views.AppView = Backbone.View.extend({
    el: $('article'),

    initialize: function (options) {
        _.extend(this, Backbone.Events);
        _.bindAll(this, 'showStreamView', 'showObservationCreateFormView');
        this.streamView = new Bowerbird.Views.StreamView();
        this.$el.append(this.streamView.render().el);
        this.formView = null;
        app.stream.on('newStream', this.showStreamView, this);
        app.on('change:newObservation', this.showObservationCreateFormView, this);
    },

    showStreamView: function () {
        if (this.formView) {
            $(this.formView.el).remove();
        }
        $(this.streamView.el).show();
        window.scrollTo(0, 0);
    },

    showObservationCreateFormView: function () {
        if (app.has('newObservation')) {
            $(this.streamView.el).hide();
            this.formView = new Bowerbird.Views.ObservationCreateFormView({ appView: this });
            this.$el.append(this.formView.render().el);
            this.formView.start();
        }
    }
});

window.Bowerbird.Views.StreamView = Backbone.View.extend({
    id: 'stream',

    initialize: function (options) {
        _.extend(this, Backbone.Events);
        this.sidebarView = new Bowerbird.Views.SidebarView();
        this.streamListView = new Bowerbird.Views.StreamListView();
    },

    render: function () {
        this.$el.append(this.sidebarView.render().el);
        this.$el.append(this.streamListView.render().el);
        return this;
    }
});

window.Bowerbird.Views.SidebarView = Backbone.View.extend({
    tagName: 'section',

    className: 'triple-1',

    id: 'sidebar',

    template: $.template('sidebarTemplate', $('#sidebar-template')),

    initialize: function (options) {
        _.extend(this, Backbone.Events);
        this.teamSidebarItemViews = [];
        this.projectSidebarItemViews = [];
        app.teams.on('add', this.addTeamSideBarItem, this);
        app.projects.on('add', this.addProjectSideBarItem, this);
    },

    render: function () {
        $.tmpl("sidebarTemplate").appendTo(this.$el);
        return this;
    },

    addTeamSideBarItem: function (team) {
        var sidebarItemView = new Bowerbird.Views.SidebarItemView({ sidebarItem: team });
        this.teamSidebarItemViews.push(sidebarItemView);
        $("#team-menu-group ul").append(sidebarItemView.render().el);
    },

    addProjectSideBarItem: function (project) {
        var sidebarItemView = new Bowerbird.Views.SidebarItemView({ sidebarItem: project });
        this.projectSidebarItemViews.push(sidebarItemView);
        $("#project-menu-group ul").append(sidebarItemView.render().el);
    }
});

window.Bowerbird.Views.SidebarItemView = Backbone.View.extend({
    tagName: 'li',

    template: $.template('sidebarItemTemplate', $('#sidebar-item-template')),

    initialize: function (options) {
        _.extend(this, Backbone.Events);
        this.sidebarItem = options.sidebarItem;
    },

    render: function () {
        $.tmpl("sidebarItemTemplate", this.sidebarItem.toJSON()).appendTo(this.$el);
        return this;
    }
});

window.Bowerbird.Views.StreamListView = Backbone.View.extend({
    tagName: 'section',

    className: 'triple-2',

    id: 'stream-list',

    events: {
        "click #stream-load-more-button": "loadNextStreamItems"
    },

    template: $.template('streamListTemplate', $('#stream-list-template')),

    loadingTemplate: $.template('streamListLoadingTemplate', $('#stream-list-loading-template')),

    loadMoreTemplate: $.template('streamLoadMoreTemplate', $('#stream-load-more-template')),

    initialize: function (options) {
        _.extend(this, Backbone.Events);
        this.streamListItemViews = [];
        app.stream.streamItems.on('add', this.addStreamItem, this);
        app.stream.on('newStream', this.showNewStream, this);
        //app.stream.on('newStreamFilter', this.showNewStreamFilter, this);
        //app.stream.on('newStreamPage', this.showNewStreamPage, this);
        app.stream.on('fetchingItemsStarted', this.showStreamLoading, this);
        app.stream.on('fetchingItemsComplete', this.hideStreamLoading, this);
    },

    render: function () {
        $.tmpl('streamListTemplate').appendTo(this.$el);
        return this;
    },

    addStreamItem: function (streamItem) {
        var streamListItemView = new Bowerbird.Views.StreamListItemView({ streamItem: streamItem });
        this.streamListItemViews.push(streamListItemView);
        $('#stream-items').append(streamListItemView.render().el);
    },

    showNewStream: function (stream) {
        $('#stream-items').empty();
        $('#stream-load-more').remove();
        if (stream.get('context') instanceof Bowerbird.Models.Team) {
        } else if (stream.get('context') instanceof Bowerbird.Models.Project) {
        } else if (stream.get('context') instanceof Bowerbird.Models.User) {
        } else {

        }
    },

    showStreamLoading: function (stream) {
        $('#stream-load-more').remove();
        $('#stream-items').append($.tmpl('streamListLoadingTemplate', { text: 'Loading', showLoader: true }));
    },

    hideStreamLoading: function (stream, collection) {
        $('#stream-status').remove();
        if (collection.length == 0) {
            $('#stream-items').append($.tmpl('streamListLoadingTemplate', { text: 'No activity yet! Start now by adding an observation.', showLoader: false }));
        }
        if (collection.pageInfo().next) {
            this.$el.append($.tmpl('streamLoadMoreTemplate'));
        }
    },

    loadNextStreamItems: function () {
        app.stream.setNextPage();
    }
});

window.Bowerbird.Views.StreamListItemView = Backbone.View.extend({
    className: 'stream-item',

    observationTemplate: $.template('observationStreamListItemTemplate', $('#observation-stream-list-item-template')),

    postTemplate: $.template('postStreamListItemTemplate', $('#post-stream-list-item-template')),

    initialize: function (options) {
        _.extend(this, Backbone.Events);
        this.streamItem = options.streamItem;
    },

    render: function () {
        var options = { 
            formatDate: function () { 
                return new Date(parseInt(this.data.Item.ObservedOn.substr(6))).format('d MMM yyyy'); 
                },
            formatTime: function () {
                return new Date(parseInt(this.data.Item.ObservedOn.substr(6))).format('h:mm');
                } 
            };
        switch (this.streamItem.get('Type')) {
            case 'Observation':
                $.tmpl('observationStreamListItemTemplate', this.streamItem.toJSON(), options).appendTo(this.$el);
                this.$el.addClass('observation-stream-item');
                break;
            default:
                break;
        }
        return this;
    }
});

window.Bowerbird.Views.ObservationCreateFormView = Backbone.View.extend({

    tagName: 'section',

    className: 'form single-medium',

    events: {
        "click #cancel": "cancel",
        "click #save": "save"
    },

    template: $.template('observationCreateFormTemplate', $('#observation-create-form-template')),

    initialize: function (options) {
        _.extend(this, Backbone.Events);
        _.bindAll(this, 'start');
        this.appView = options.appView;
        this.uploader = null;
        this.mediaResources = new Array();
    },

    render: function () {
        $.tmpl('observationCreateFormTemplate', app.get('newObservation').toJSON()).appendTo(this.$el);
        window.scrollTo(0, 0);
        return this;
    },

    start: function () {
        buildMap();
        buildMediaUploader('observation', true);
    },

    cancel: function () {
        app.set('newObservation', null);
        this.$el.remove();
        this.appView.showStreamView();
        app.router.navigate(app.stream.get('uri'), { trigger: true });
    },

    save: function () {
        alert('not yet!');
        //this.model.set({
        //    "title": $("#title").attr("value"),
        //    "address": $("#address").attr("value"),
        //    "latitude": $("#latitude").attr("value"),
        //    "longitude": $("#longitude").attr("value"),
        //    "observedOn": $("#observedOn").attr("value"),
        //    "isIdentificationRequired": $("#isIdentificationRequired").attr("value"),
        //    "observationCategory": $("#observationCategory").attr("value")
        //});

        ////window.observations.add(this.model);

        //this.model.save();

        //this.hide($(this.el));
    }

});

/*************************************************************
Other
*************************************************************/

window.Bowerbird.Router = Backbone.Router.extend({
    routes: {
        '': 'showDefaultStream',
        'home/:filter': 'showHomeStream',
        'teams/:id/:filter': 'showTeamStream',
        'projects/:id/:filter': 'showProjectStream',
        'users/:id/:filter': 'showUserStream',
        'observation/create': 'startNewObservation'
    },

    initialize: function (options) {
        _.extend(this, Backbone.Events);
    },

    showDefaultStream: function () {
        app.showHomeStream('all');
    },

    showHomeStream: function (filter) {
        app.showHomeStream(filter);
    },

    showTeamStream: function (id, filter) {
        app.showTeamStream(id, filter);
    },

    showProjectStream: function (id, filter) {
        app.showProjectStream(id, filter);
    },

    showUserStream: function (id, filter) {

    },

    startNewObservation: function () {
        app.startNewObservation();
    }
});

window.Bowerbird.App = Backbone.Model.extend({
    defaults: {
        newObservation: null
    },

    initialize: function (options) {
        this.teams = new Bowerbird.Collections.Teams();
        this.projects = new Bowerbird.Collections.Teams();
        this.stream = new Bowerbird.Models.Stream();
    },

    showHomeStream: function (filter) {
        this.stream.setNewStream(null, filter);
    },

    showTeamStream: function (id, filter) {
        this.stream.setNewStream(this.teams.get('teams/' + id), filter);
    },

    showProjectStream: function (id, filter) {
        this.stream.setNewStream(this.projects.get('projects/' + id), filter);
    },

    showUserStream: function (user, filter) {

    },

    startNewObservation: function () {
        this.set('newObservation', new Bowerbird.Models.Observation());
    },

    cancelNewObservation: function () {
        this.set('newObservation', null);
    }
});

function startBowerbird(teams, projects) {
    window.app = new Bowerbird.App();
    app.appView = new Bowerbird.Views.AppView();
    app.appView.showStreamView();
    app.router = new Bowerbird.Router();
    Backbone.history.start({ pushState: false });
    app.teams.add(teams);
    app.projects.add(projects);
}

/*************************************************************
Map related, to be folded into views above
*************************************************************/

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