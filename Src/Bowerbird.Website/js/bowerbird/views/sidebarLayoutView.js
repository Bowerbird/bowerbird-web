    /// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// SidebarLayoutView
// -----------------

// The left hand side bar that is shown to authenticated users.
define(['jquery', 'underscore', 'backbone', 'app', 'views/sidebarmenugroupcompositeview', 'views/sidebarprojectitemview', 'views/sidebarteamitemview', 'views/sidebarorganisationitemview'],
function ($, _, Backbone, app, SidebarMenuGroupCompositeView, SidebarProjectItemView, SidebarTeamItemView, SidebarOrganisationItemView) {

    var SidebarLayoutView = Backbone.Marionette.Layout.extend({
        tagName: 'section',

        id: 'sidebar',

        className: 'triple-1',

        template: 'Sidebar',

        regions: {
            projectsMenu: '.menu-projects',
            watchlistsMenu: '#watch-menu-group #watch-list',
            teamsMenu: '.menu-teams',
            organisationsMenu: '.menu-organisations'
        },

        events: {
            'click .menu-group-options .sub-menu-button': 'showMenu',
            'click .menu-group-options .sub-menu-button li': 'selectMenuItem'
        },

        onRender: function () {
            $('article').prepend(this.el);

            var sidebarProjectCompositeView = new SidebarMenuGroupCompositeView({ id: 'project-menu-group', collection: this.model.projects, type: 'project', label: 'Projects' });
            sidebarProjectCompositeView.itemView = SidebarProjectItemView;
            this.projectsMenu.show(sidebarProjectCompositeView);

            if (this.model.teams.length > 0) {
                var sidebarTeamCompositeView = new SidebarMenuGroupCompositeView({ id: 'team-menu-group', collection: this.model.teams, type: 'team', label: 'Teams' });
                sidebarTeamCompositeView.itemView = SidebarTeamItemView;
                this.teamsMenu.show(sidebarTeamCompositeView);
            }

            if (this.model.organisations.length > 0) {
                var sidebarOrganisationCompositeView = new SidebarMenuGroupCompositeView({ id: 'organisation-menu-group', collection: this.model.organisations, type: 'organisation', label: 'Organisations' });
                sidebarOrganisationCompositeView.itemView = SidebarOrganisationItemView;
                this.organisationsMenu.show(sidebarOrganisationCompositeView);
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
                var sidebarLayoutView = new SidebarLayoutView({ model: model });

                sidebarLayoutView.on('show', function () {
                    app.vent.trigger('sidebar:rendered');
                });

                app.sidebar.show(sidebarLayoutView);
            }
        });
    });

    return SidebarLayoutView;

});