
window.Bowerbird.Views.SidebarView = Backbone.View.extend({
    tagName: 'section',

    className: 'triple-1',

    id: 'sidebar',

    template: $.template('sidebarTemplate', $('#sidebar-template')),

    initialize: function (options) {
        _.extend(this, Backbone.Events);
        _.bindAll(this, 'addTeamSideBarItem', 'addProjectSideBarItem', 'addTeamSideBarItems', 'addProjectSideBarItems');
        this.teamSidebarItemViews = [];
        this.projectSidebarItemViews = [];
        app.teams.on('add', this.addTeamSideBarItem, this);
        app.projects.on('add', this.addProjectSideBarItem, this);
        app.teams.on('reset', this.addTeamSideBarItems, this);
        app.projects.on('reset', this.addProjectSideBarItems, this);
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

    addProjectSideBarItem: function (project, x, y, z) {
        var sidebarItemView = new Bowerbird.Views.SidebarItemView({ sidebarItem: project });
        this.projectSidebarItemViews.push(sidebarItemView);
        $("#project-menu-group ul").append(sidebarItemView.render().el);
    },

    addTeamSideBarItems: function (teams) {
        teams.each(this.addTeamSideBarItem);
    },

    addProjectSideBarItems: function (projects) {
        projects.each(this.addProjectSideBarItem);
    }
});