    /// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// SidebarView
// -----------

// The left hand side bar that is shown to authenticated users.
define(['jquery', 'underscore', 'backbone', 'app', 'views/sidebarmenugroupview', 'views/sidebarprojectitemview', 'views/sidebarorganisationitemview', 'views/sidebaruserprojectitemview', 'tipsy'],
function ($, _, Backbone, app, SidebarMenuGroupView, SidebarProjectItemView, SidebarOrganisationItemView, SidebarUserProjectItemView) {

    var SidebarView = Backbone.Marionette.Layout.extend({
        tagName: 'section',

        id: 'sidebar',

        className: 'double-1',

        template: 'Sidebar',

        regions: {
            projectsMenu: '.menu-projects',
            watchlistsMenu: '#watch-menu-group #watch-list',
            organisationsMenu: '.menu-organisations',
            userProjectsMenu: '.menu-userprojects'
        },

        events: {
            'click #default-menu-group .sub-menu': 'showMenu',
            'click #default-menu-group .sub-menu a': 'selectMenuItem',
            'click #action-menu a': 'selectMenuItem',
            'click #default-menu-group .menu-group-item > a': 'selectMenuItem',
            'click .more-button': 'showMoreMenu',
            'click .menu-groups-tabs li': 'showMenuGroup'
        },

        onRender: function () {
            $('article').prepend(this.el);

            var sidebarProjectMenuGroupView = new SidebarMenuGroupView({ id: 'project-menu-group', collection: this.model.projects, type: 'project', label: 'Projects' });
            sidebarProjectMenuGroupView.itemView = SidebarProjectItemView;
            this.projectsMenu.show(sidebarProjectMenuGroupView);

            var sidebarOrganisationMenuGroupView = new SidebarMenuGroupView({ id: 'organisation-menu-group', collection: this.model.organisations, type: 'organisation', label: 'Organisations' });
            sidebarOrganisationMenuGroupView.itemView = SidebarOrganisationItemView;
            this.organisationsMenu.show(sidebarOrganisationMenuGroupView);

            var sidebarUserProjectMenuGroupView = new SidebarMenuGroupView({ id: 'user-menu-group', collection: this.model.userProjects, type: 'userproject', label: 'People' });
            sidebarUserProjectMenuGroupView.itemView = SidebarUserProjectItemView;
            this.userProjectsMenu.show(sidebarUserProjectMenuGroupView);

            this.currentMenuGroupView = sidebarProjectMenuGroupView;

            this.$el.find('#action-menu a').not('.new-observation-button').tipsy({ gravity: 'n', html: true });
            this.$el.find('.new-observation-button').tipsy({ gravity: 'nw', html: true });
            this.$el.find('#default-menu-group .sub-menu a').tipsy({ gravity: 'w' });

            app.vent.on('newactivity:sightingadded newactivity:postadded newactivity:sightingnoteadded', this.onNewActivityReceived, this);
        },

        serializeData: function () {
            return {
                Model: {
                    User: this.model.user.toJSON(),
                    Projects: this.model.projects.length > 0,
                    Organisations: this.model.organisations.length > 0,
                    Users: this.model.userProjects.length > 0,
                    AppRoot: this.model.appRoot != null
                }
            };
        },

        showMenu: function (e) {
            app.vent.trigger('close-sub-menus');
            $(e.currentTarget).addClass('active');
            e.stopPropagation();
        },

        showMoreMenu: function (e) {
            app.vent.trigger('close-sub-menus');
            var left = $(e.currentTarget).parent().width() - $(e.currentTarget).width();
            $(e.currentTarget).find('ul').css({ left: '-' + left + 'px' });
            $(e.currentTarget).addClass('active');
            e.stopPropagation();
        },

        selectMenuItem: function (e) {
            e.preventDefault();
            app.vent.trigger('close-sub-menus');
            Backbone.history.navigate($(e.currentTarget).attr('href'), { trigger: true });
            return false;
        },

        showMenuGroup: function (e) {
            e.preventDefault();
            var menu = $(e.currentTarget).data('menu');
            this.$el.find('.menu-groups-tabs li').removeClass('active');
            $(e.currentTarget).addClass('active');

            this.$el.find('.menu-groups > div').hide();
            this.$el.find('.' + menu).show();

            var scrollbar = this.$el.find('.' + menu).find('.scrollbar-container');
            scrollbar.tinyscrollbar_update();

            $(e.currentTarget).find('span').remove();

            return false;
        },

        onNewActivityReceived: function (activity) {
            _.each(activity.get('Groups'), function (group) {
                if (group.Id.toLowerCase().indexOf('projects/', 0) == 0 && this.$el.find('.menu-projects-tab.active').length == 0) {
                    this.showActivityBadge('.menu-projects-tab');
                }
                if (group.Id.toLowerCase().indexOf('userprojects/', 0) == 0 && this.$el.find('.menu-userprojects-tab.active').length == 0) {
                    this.showActivityBadge('.menu-userprojects-tab');
                }
                if (group.Id.toLowerCase().indexOf('organisations/', 0) == 0 && this.$el.find('.menu-organisations-tab.active').length == 0) {
                    this.showActivityBadge('.menu-organisations-tab');
                }
            },
            this);
        },

        showActivityBadge: function (tab) {
            this.$el.find(tab).append('<span title="New Items">New</span>');
        }
    });

    // Initialize the sidebar layout
    app.addInitializer(function (options) {
        $(function () {
            // Only show sidebar if user is authenticated
            if (app.authenticatedUser) {
                var model = {
                    user: app.authenticatedUser.user,
                    projects: app.authenticatedUser.projects,
                    organisations: app.authenticatedUser.organisations,
                    userProjects: app.authenticatedUser.userProjects,
                    appRoot: app.authenticatedUser.appRoot
                };

                // Render the layout and get it on the screen, first
                var sidebarView = new SidebarView({ model: model });

                sidebarView.on('show', function () {
                    app.vent.trigger('sidebar:rendered');
                });

                sidebarView.$el.hide();

                app.sidebar.show(sidebarView);
            }
        });
    });

    return SidebarView;

});