/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// SidebarLayoutView
// -----------------

// The left hand side bar that is shown to authenticated users.
define(['jquery', 'underscore', 'backbone', 'app', 'views/sidebarprojectcollectionview', 'views/sidebarteamcollectionview', 'views/sidebarorganisationcollectionview'],
function ($, _, Backbone, app, SidebarProjectCollectionView, SidebarTeamCollectionView, SidebarOrganisationCollectionView) {

    var SidebarLayoutView = Backbone.Marionette.Layout.extend({
        tagName: 'section',

        id: 'sidebar',

        className: 'triple-1',

        template: 'Sidebar',

        regions: {
            projectsMenu: '#project-menu-group #project-list',
            watchlistsMenu: '#watch-menu-group #watch-list',
            teamsMenu: '.menu-teams',
            organisationsMenu: '.menu-organisations'
            //appRootMenu: '.menu-approot'
        },

        events: {
            'click .menu-group-options .sub-menu-button': 'showMenu',
            'click .menu-group-options .sub-menu-button li': 'selectMenuItem'
        },

        onRender: function () {
            $('article').prepend(this.el);

            var sidebarProjectCollectionView = new SidebarProjectCollectionView({ collection: this.model.projects });
            this.projectsMenu.show(sidebarProjectCollectionView);

            if (this.model.teams.length > 0) {
                var sidebarTeamCollectionView = new SidebarTeamCollectionView({ collection: this.model.teams });
                this.teamsMenu.show(sidebarTeamCollectionView);
            }

            if (this.model.organisations.length > 0) {
                var sidebarOrganisationCollectionView = new SidebarOrganisationCollectionView({ collection: this.model.organisations });
                this.organisationsMenu.show(sidebarOrganisationCollectionView);
            }

            var that = this;
            this.$el.find('a.user-stream').on('click', function (e) {
                e.preventDefault();
                app.groupUserRouter.navigate($(this).attr('href'), { trigger: true });
                //app.vent.trigger('home:show');
                return false;
            });
            this.$el.find('#project-menu-group-list .menu-group-options a').on('click', function (e) {
                e.preventDefault();
                app.projectRouter.navigate($(this).attr('href'), { trigger: true });
                //app.vent.trigger('home:show');
                return false;
            });
            this.$el.find('#team-menu-group-list .menu-group-options a').on('click', function (e) {
                e.preventDefault();
                app.teamRouter.navigate($(this).attr('href'), { trigger: true });
                //app.vent.trigger('home:show');
                return false;
            });
            this.$el.find('#organisation-menu-group-list .menu-group-options a').on('click', function (e) {
                e.preventDefault();
                app.organisationRouter.navigate($(this).attr('href'), { trigger: true });
                //app.vent.trigger('home:show');
                return false;
            });
        },

        serializeData: function () {
            return {
                User: this.model.user,
                Teams: this.model.teams.length > 0,
                Organisations: this.model.organisations.length > 0,
                AppRoot: this.model.appRoot != null
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
        }
    });

    // Initialize the sidebar layout
    app.addInitializer(function (options) {
        // Only show sidebar if user is authenticated
        if (this.user) {
            var model = {
                user: this.user,
                projects: this.projects,
                teams: this.teams,
                organisations: this.organisations,
                appRoot: this.appRoot
            };

            // Render the layout and get it on the screen, first
            var sidebarLayoutView = new SidebarLayoutView({ model: model });

            sidebarLayoutView.on('show', function () {
                app.vent.trigger('sidebar:rendered');
            });

            app.sidebar.show(sidebarLayoutView);
        }
    });

    return SidebarLayoutView;

});