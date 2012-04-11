
window.Bowerbird.Views.SidebarView = Backbone.View.extend({
    tagName: 'section',

    className: 'triple-1',

    id: 'sidebar',

    events: {
        'click .menu-group-options .sub-menu-button': 'showMenu',
        'click .menu-group-options .sub-menu-button li': 'selectMenuItem'
    },

    initialize: function (options) {
        _.extend(this, Backbone.Events);
        _.bindAll(this,
        'addTeamSideBarItem',
        'addProjectSideBarItem',
        'addTeamSideBarItems',
        'addProjectSideBarItems'
        );
        this.teamSidebarItemViews = [];
        this.projectSidebarItemViews = [];
        app.teams.on('add', this.addTeamSideBarItem, this);
        app.projects.on('add', this.addProjectSideBarItem, this);
        app.teams.on('reset', this.addTeamSideBarItems, this);
        app.projects.on('reset', this.addProjectSideBarItems, this);
    },

    render: function () {
        
        var sidebarHtml = ich.sidebar().appendTo(this.$el);

        this.notificationsView = new Bowerbird.Views.NotificationsView();
        this.$el.append(this.notificationsView.render().el);

//        this.$el.find('.menu-group-options .sub-menu-button').click(function (e) {
//            $('.sub-menu-button').removeClass('active');
//            $(this).addClass('active');
//        });

//        this.$el.find('.menu-group-options .sub-menu-button').click(function (e) {
//            e.stopPropagation();
//        });

        return this;
    },

    showMenu: function (e) {
        $('.sub-menu-button').removeClass('active');
        $(e.currentTarget).addClass('active');
        e.stopPropagation();
    },

    selectMenuItem: function (e) {
        $('.sub-menu-button').removeClass('active');
        e.stopPropagation();
    },

    addTeamSideBarItem: function (team) {
        var sidebarItemView = new Bowerbird.Views.SidebarItemView({ sidebarItem: team, type: 'Team' });
        this.teamSidebarItemViews.push(sidebarItemView);
        $("#team-menu-group > ul .menu-group-options").before(sidebarItemView.render().el);
    },

    addProjectSideBarItem: function (project, x, y, z) {
        var sidebarItemView = new Bowerbird.Views.SidebarItemView({ sidebarItem: project, type: 'Project' });
        this.projectSidebarItemViews.push(sidebarItemView);
        $("#project-menu-group > ul .menu-group-options").before(sidebarItemView.render().el);
    },

    addTeamSideBarItems: function (teams) {
        teams.each(this.addTeamSideBarItem);
    },

    addProjectSideBarItems: function (projects) {
        projects.each(this.addProjectSideBarItem);
    }
});