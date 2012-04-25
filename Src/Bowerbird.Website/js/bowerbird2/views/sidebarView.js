/// <reference path="../libs/log.js" />
/// <reference path="../libs/jquery-1.7.1.min.js" />
/// <reference path="../libs/underscore.js" />
/// <reference path="../libs/backbone.js" />
/// <reference path="../libs/backbone.marionette.js" />
/// <reference path="namespace.js" />
/// <reference path="app.js" />

// Bowerbird.Views.SidebarView
// -----------------------

Bowerbird.Views.SidebarView = (function (app, Bowerbird, Backbone, $, _) {

    var SidebarView = Backbone.View.extend({
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
            'addProjectSideBarItems');
            this.teamSidebarItemViews = [];
            this.projectSidebarItemViews = [];
            app.teams.on('add', this.addTeamSideBarItem, this);
            app.projects.on('add', this.addProjectSideBarItem, this);
            app.teams.on('reset', this.addTeamSideBarItems, this);
            app.projects.on('reset', this.addProjectSideBarItems, this);
        },

        render: function () {
            this.$el.append(ich.Sidebar());

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

    return SidebarView;

})(Bowerbird.app, Bowerbird, Backbone, jQuery, _);