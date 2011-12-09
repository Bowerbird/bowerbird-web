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
        username: ''
    }

});

// Team
window.Bowerbird.Models.Team = Backbone.Model.extend({

    defaults: {
        key: '',
        name: '',
        description: ''
    }

});

// Project
window.Bowerbird.Models.Project = Backbone.Model.extend({

    defaults: {
        key: '',
        name: '',
        description: ''
    }

});

// Watch
window.Bowerbird.Models.Watch = Backbone.Model.extend({

    defaults: {
        key: '',
        name: '',
        description: ''
    }

});

// Observation
window.Bowerbird.Models.Observation = Backbone.Model.extend({

    defaults: {
        key: "",
        title: '',
        commonName: '',
        scientificName: '',
        description: '',
        latitude: null,
        longitude: null
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

    url: "/observation"

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

    el: '#sidebar-column',

    initialize: function (options) {
        _.extend(this, Backbone.Events);
    }

});

// Workspace Container
window.Bowerbird.Views.WorkspaceContainer = Backbone.View.extend({

    el: '#workspace-column',

    initialize: function (options) {
        _.extend(this, Backbone.Events);
        this.workspaces = [];
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
        var streamWorkspace = new Bowerbird.Views.StreamWorkspace({ id: "stream-" + type + "-" + key, model: stream });

        streamWorkspace.bind("newObservation", this.showObservationForm, this);

        this.showWorkspace(streamWorkspace);
    },

    showObservationForm: function (model) {
        var observation = new Bowerbird.Models.Observation();
        var formWorkspace = new Bowerbird.Views.FormWorkspace({ id: 'form-create-observation', model: observation });
        this.showWorkspace(formWorkspace);
    },

    showWorkspace: function (workspace) {
        // Hide current workspace if it is a stream
        if (workspace instanceof Bowerbird.Views.StreamWorkspace) {
            while (this.workspaces.length > 0) {
                var currentWorkspace = this.workspaces.shift();
                currentWorkspace.hide($(currentWorkspace.el));
            }
        }

        // Show new workspace
        this.workspaces.unshift(workspace);
        workspace.render();

        workspace.loadEvents();
    }

});

// Workspace
window.Bowerbird.Views.Workspace = Backbone.View.extend({

    renderWorkspace: function (workspaceElement, html) {
        workspaceElement.html(html);
        workspaceElement.appendTo("#workspace-column");

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

// Stream Workspace
window.Bowerbird.Views.StreamWorkspace = Bowerbird.Views.Workspace.extend({

    className: 'workspace-item workspace-stream',

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
        this.renderWorkspace($(this.el), compiledHtml);
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

// Form Workspace
window.Bowerbird.Views.FormWorkspace = Bowerbird.Views.Workspace.extend({

    className: 'workspace-item workspace-stream',

    events: {
        "click #cancel": "cancel",
        "click #save": "save"
    },

    initialize: function (options) {
        _.extend(this, Backbone.Events);
    },

    render: function () {
        var compiledHtml = $.tmpl($("#workspace-form-create-observation-template"), this.model.toJSON());
        this.renderWorkspace($(this.el), compiledHtml);
        return this;
    },

    cancel: function () {
        this.hide($(this.el));
    },

    save: function () {
        this.model.set({ "title": $("#title").attr("value"), "description": "hello", "address": "hello" });

        window.observations.add(this.model);

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
        "/team/:key/stream/:filter": "showTeamStream",
        "/project/:key/stream/:filter": "showProjectStream",
        "/watch/:key/stream/:filter": "showWatchStream"
        //"search/:query/p:page": "search"   // #search/kiwis/p7
    },

    initialize: function (options) {
        this.header = new Bowerbird.Views.Header({ app: this });
        this.navigation = new Bowerbird.Views.Navigation({ app: this });
        this.workspaceContainer = new Bowerbird.Views.WorkspaceContainer({ app: this });
    },

    showUserStream: function (filter) {
        this.workspaceContainer.showStream("user", "user", filter);
    },

    showTeamStream: function (key, filter) {
        this.workspaceContainer.showStream("team", key, filter);
    },

    showProjectStream: function (key, filter) {
        this.workspaceContainer.showStream("project", key, filter);
    },

    showWatchStream: function (key, filter) {
        this.workspaceContainer.showStream("watch", key, filter);
    }

    //    search: function (query, page) {

    //    }

});
