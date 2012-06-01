/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// SidebarLayoutView
// -----------------

// The left hand side bar that is shown to authenticated users.
define([
'jquery',
'underscore',
'backbone',
'app',
'models/user',
'views/sidebarprojectcollectionview', 
'views/sidebarteamcollectionview',
'views/sidebarorganisationcollectionview',
'collections/projectcollection',
'collections/teamcollection', 
'collections/organisationcollection',
'models/project',
'models/team', 
'models/organisation'],
function (
$,
_,
Backbone,
app,
User,
SidebarProjectCollectionView,
SidebarTeamCollectionView,
SidebarOrganisationCollectionView,
ProjectCollection,
TeamCollection,
OrganisationCollection,
Project,
Team, 
Organisation) 
{
        var SidebarLayoutView = Backbone.Marionette.Layout.extend({
            tagName: 'section',

            id: 'sidebar',

            className: 'triple-1',

            template: 'Sidebar',

            regions: {
                projectsMenu: '#project-menu-group',
                teamsMenu: '#team-menu-group',
                organisationsMenu: '#organisation-menu-group',
                watchlistsMenu: '#watch-menu-group > ul'
            },

            events: {
                'click .menu-group-options .sub-menu-button': 'showMenu',
                'click .menu-group-options .sub-menu-button li': 'selectMenuItem'
            },

            onRender: function () {
                $('article').prepend(this.el);

                var sidebarProjectCollectionView = new SidebarProjectCollectionView({ el: '#project-menu-group-list', collection: this.model.userProjects });
                this.projectsMenu.attachView(sidebarProjectCollectionView);

                var sidebarTeamCollectionView = new SidebarTeamCollectionView({ el: '#team-menu-group-list', collection: this.model.teams });
                this.teamsMenu.attachView(sidebarTeamCollectionView);

                var sidebarOrganisationCollectionView = new SidebarOrganisationCollectionView({ el: '#organisation-menu-group-list', collection: this.model.organisations });
                this.organisationsMenu.attachView(sidebarOrganisationCollectionView);

                sidebarProjectCollectionView.render();
                sidebarTeamCollectionView.render();
                sidebarOrganisationCollectionView.render();

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
                    User: this.model.user
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
                // Render the layout and get it on the screen, first
                var sidebarLayoutView = new SidebarLayoutView({ model: { user: this.user, userProjects: this.userProjects, teams: this.teams, organisations: this.organisations } });

                sidebarLayoutView.on('show', function () {
                    app.vent.trigger('sidebar:rendered');
                });

                app.sidebar.show(sidebarLayoutView);
            }
        });

        return SidebarLayoutView;

    });