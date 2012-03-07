
window.Bowerbird.Views.SidebarView = Backbone.View.extend({
    tagName: 'section',

    className: 'triple-1',

    id: 'sidebar',

    events: {
        'click #start-chat': 'startChat'
    },

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
    },

    startChat: function () {
        var chat = new Bowerbird.Models.Chat({ group: app.teams.get('teams/1') });
        app.chats.add(chat);
    }
});