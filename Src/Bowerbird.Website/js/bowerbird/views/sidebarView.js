    /// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// SidebarView
// -----------

// The left hand side bar that is shown to authenticated users.
define(['jquery', 'underscore', 'backbone', 'app', 'views/sidebarmenugroupview', 'views/sidebarprojectitemview', 'views/sidebarteamitemview', 'views/sidebarorganisationitemview'],
function ($, _, Backbone, app, SidebarMenuGroupView, SidebarProjectItemView, SidebarTeamItemView, SidebarOrganisationItemView) {

    var SidebarView = Backbone.Marionette.Layout.extend({
        tagName: 'section',

        id: 'sidebar',

        className: 'double-1',

        template: 'Sidebar',

        regions: {
            projectsMenu: '.menu-projects',
            watchlistsMenu: '#watch-menu-group #watch-list',
            teamsMenu: '.menu-teams',
            organisationsMenu: '.menu-organisations'
        },

        events: {
            'click #default-menu-group .sub-menu-button': 'showMenu'
            //'click .menu-group-options .sub-menu-button li': 'selectMenuItem'
        },

        onRender: function () {
            $('article').prepend(this.el);

            var sidebarProjectMenuGroupView = new SidebarMenuGroupView({ id: 'project-menu-group', collection: this.model.projects, type: 'project', label: 'Projects' });
            sidebarProjectMenuGroupView.itemView = SidebarProjectItemView;
            this.projectsMenu.show(sidebarProjectMenuGroupView);

            if (this.model.teams.length > 0) {
                var sidebarTeamMenuGroupView = new SidebarMenuGroupView({ id: 'team-menu-group', collection: this.model.teams, type: 'team', label: 'Teams' });
                sidebarTeamMenuGroupView.itemView = SidebarTeamItemView;
                this.teamsMenu.show(sidebarTeamMenuGroupView);
            }

            if (this.model.organisations.length > 0) {
                var sidebarOrganisationMenuGroupView = new SidebarMenuGroupView({ id: 'organisation-menu-group', collection: this.model.organisations, type: 'organisation', label: 'Organisations' });
                sidebarOrganisationMenuGroupView.itemView = SidebarOrganisationItemView;
                this.organisationsMenu.show(sidebarOrganisationMenuGroupView);
            }

            this.$el.find('#action-menu a, #default-menu-group a').on('click', function (e) {
                e.preventDefault();
                Backbone.history.navigate($(this).attr('href'), { trigger: true });
                return false;
            });

            app.authenticatedUser.projects.on('add', this.addProject, this);

            app.authenticatedUser.projects.on('remove', this.removeProject, this);
        },

        serializeData: function () {
            return {
                Model: {
                    User: this.model.user.toJSON(),
                    Teams: this.model.teams.length > 0,
                    Organisations: this.model.organisations.length > 0,
                    AppRoot: this.model.appRoot != null
                }
            };
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

        addProject: function (project) {
            log('project added', project, this);
        },

        removeProject: function (project) {
            log('project removed', project, this);
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
                    teams: app.authenticatedUser.teams,
                    organisations: app.authenticatedUser.organisations,
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