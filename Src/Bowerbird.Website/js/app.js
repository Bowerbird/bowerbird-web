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
        type: null,
        context: null,
        filter: null
    },

    initialize: function (options) {
        _.extend(this, Backbone.Events);
        _.bindAll(this);
        this.streamItems = new Bowerbird.Collections.StreamItems();
    },

    getHomeStreamItems: function (filter) {
        this.trigger('fetchingItemsStarted');
        this._setStream('home', null, filter);
        this._fetchStreamItems(false);
    },

    getTeamStreamItems: function (team, filter) {
        this._setStream('team', team, filter);
        this._fetchStreamItems(false);
    },

    getProjectStreamItems: function (projects, filter) {
        this._setStream('project', project, filter);
        this._fetchStreamItems(false);
    },

    getUserStreamItems: function (user, filter) {
        this._setStream('user', user, filter);
        this._fetchStreamItems(false);
    },

    getNewFilterStreamItems: function (filter) {
        this.filter = filter;
        this._fetchStreamItems(false);
    },

    _setStream: function (type, context, filter) {
        this.type = type;
        this.context = context;
        this.filter = filter;
    },

    _getFetchData: function () {
        var data = {};
        switch (this.type) {
            case 'team':
            case 'project':
                data.groupId = this.context.id;
                break;
            case 'user':
                data.userId = this.context.id;
                break;
            default:
                break;
        }
        if (this.filter != null) {
            data.filter = this.filter;
        }
    },

    getNextStreamItems: function () {
        var data = this._getFetchData();
        var self = this;
        this.streamItems.nextPage({
            data: data,
            add: true,
            success: function (collection, response) {
                self.trigger('fetchingItemsComplete', response);
                // Added the following manual triggering of 'add' event due to Backbone bug: https://github.com/documentcloud/backbone/issues/479
                response.forEach(function (item) {
                    app.stream.streamItems.trigger('add', item);
                });
            }
        });
    },

    _fetchStreamItems: function (add) {
        var data = {};
        switch (this.type) {
            case 'team':
            case 'project':
                data.groupId = this.context.id;
                break;
            case 'user':
                data.userId = this.context.id;
                break;
            default:
                break;
        }
        if (this.filter != null) {
            data.filter = this.filter;
        }
        var self = this;
        this.streamItems.fetch({
            data: data,
            add: add,
            success: function (collection, response) {
                self.trigger('fetchingItemsComplete', response);
                // Added the following manual triggering of 'add' event due to Backbone bug: https://github.com/documentcloud/backbone/issues/479
                response.forEach(function (item) {
                    app.stream.streamItems.trigger('add', item);
                });
            }
        });
    }
});

window.Bowerbird.Models.StreamItem = Backbone.Model.extend({
});

window.Bowerbird.Models.Team = Backbone.Model.extend({
});

window.Bowerbird.Models.Project = Backbone.Model.extend({
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

    nextPage: function (options) {
        if (!this.pageInfo().next) {
            return false;
        }
        this.page = this.page + 1;
        return this.fetch(options);
    },

    previousPage: function () {
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
        Bowerbird.Collections.PaginatedCollection.prototype.initialize.apply(this, arguments);
        this.pageSize = 10;
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
        this.streamView = new Bowerbird.Views.StreamView();
        this.formView = null;
        this.render();
    },

    render: function () {
        this.$el.append(this.streamView.render().el);
        return this;
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
        app.stream.on('fetchingItemsStarted', this.showLoading, this);
        app.stream.on('fetchingItemsComplete', this.hideLoading, this);
    },

    render: function () {
        $.tmpl('streamListTemplate').appendTo(this.$el);
        return this;
    },

    addStreamItem: function (streamItem) {
        var streamListItemView = new Bowerbird.Views.StreamListItemView({ streamItem: streamItem });
        this.streamListItemViews.push(streamListItemView);
        $('#stream-items').prepend(streamListItemView.render().el);
    },

    showLoading: function () {
        $('#stream-items').empty().append($.tmpl('streamListLoadingTemplate', { text: 'Loading', showLoader: true }));
    },

    hideLoading: function (collection) {
        $('#stream-items').empty();
        if (collection.length == 0) {
            $('#stream-items').append($.tmpl('streamListLoadingTemplate', { text: 'No activity yet! Start now by adding an observation.', showLoader: false }));
        }
        if (collection.pageInfo().next) {
            $('#stream-items').append($.tmpl('streamLoadMoreTemplate'));
        }
    },

    loadNextStreamItems: function () {
        app.stream.getNextStreamItems();
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

/*************************************************************
Other
*************************************************************/

window.Bowerbird.Router = Backbone.Router.extend({
    routes: {
        '': 'showHomeStream',
        '/home/:filter': 'showHomeStream',
        '/teams/:id/:filter': 'showTeamStream',
        '/projects/:id/:filter': 'showProjectStream',
        '/users/:id/:filter': 'showUserStream',
        '/observation/create': 'showObservationCreateForm'
    },

    initialize: function (options) {
        _.extend(this, Backbone.Events);
    },

    showHomeStream: function (filter) {
        app.stream.getHomeStreamItems(filter);
    },

    showTeamStream: function (id, filter) {
        app.stream.getTeamStreamItems(app.teams.get('teams/' + id), filter);
    },

    showProjectStream: function (id, filter) {
        app.stream.getTeamStreamItems(app.projects.get('projects/' + id), filter);
    },

    showUserStream: function (id, filter) {
        app.stream.getUserStreamItems('users/' + id, filter);
    },

    showObservationCreateForm: function () {

    }
});

window.Bowerbird.App = Backbone.Model.extend({
    initialize: function (options) {
        this.teams = new Bowerbird.Collections.Teams();
        this.projects = new Bowerbird.Collections.Teams();
        this.stream = new Bowerbird.Models.Stream();
    },

    start: function (teams, projects) {
        this.appView = new Bowerbird.Views.AppView();
        this.teams.add(teams);
        this.projects.add(projects);
        this.router = new Bowerbird.Router();
        Backbone.history.start({ pushState: false });
    }
});