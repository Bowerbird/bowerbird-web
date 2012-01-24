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

// Workspace
window.Bowerbird.Views.Workspace = Backbone.View.extend({

    el: '#workspace',

    initialize: function (options) {
        _.extend(this, Backbone.Events);
        this.workspaceItems = [];
    },

    showStream: function (type, key, filter) {
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

        this.showWorkspaceItem(streamWorkspaceItem);
    },

    showObservationForm: function () {
        var observation = new Bowerbird.Models.Observation();
        var formWorkspaceItem = new Bowerbird.Views.FormWorkspaceItem({ id: 'form-create-observation', model: observation });
        this.showWorkspaceItem(formWorkspaceItem);
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
        workspaceElement.appendTo("#workspace");

        workspaceElement
            .css({ zIndex: 98 })
            .animate({
                left: parseInt(workspaceElement.css('left'), 10) == 0 ? -workspaceElement.outerWidth() : 0
            },
            500);
    },

    hide: function (workspaceElement) {
        var that = this;
        workspaceElement
            .css({ zIndex: 98 })
            .animate({
                left: parseInt(workspaceElement.css('left'), 10) == 0 ? -workspaceElement.outerWidth() : 0
                //opacity: 0
            },
            300,
            function () {
                that.remove();
            });
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

    className: 'workspace-item form-workspace-item',

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
        this.hide($(this.el));
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
    }

    //    showUserStream: function (filter) {
    //        this.workspaceContainer.showStream("user", "user", filter);
    //    },

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
