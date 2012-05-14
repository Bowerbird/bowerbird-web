/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// SidebarLayoutView
// -----------------

// The left hand side bar that is shown to authenticated users.
define(['jquery', 'underscore', 'backbone', 'app', 'models/user', 'views/sidebarprojectcollectionview', 'collections/projectcollection',
    'models/project'], function ($, _, Backbone, app, User, SidebarProjectCollectionView, ProjectCollection, Project) {

        var SidebarLayoutView = Backbone.Marionette.Layout.extend({
            tagName: 'section',

            id: 'sidebar',

            className: 'triple-1',

            template: 'Sidebar',

            regions: {
                projectsMenu: '#project-menu-group',
                watchlistsMenu: '#watch-menu-group > ul'
            },

            events: {
                'click .menu-group-options .sub-menu-button': 'showMenu',
                'click .menu-group-options .sub-menu-button li': 'selectMenuItem'
            },

            onRender: function () {
                $('article').prepend(this.el);

                var projects = new ProjectCollection();
                _.each(this.model.get('Projects'), function (json) {
                    projects.add(new Project(json));
                });
                var sidebarProjectCollectionView = new SidebarProjectCollectionView({ el: '#project-menu-group-list', collection: projects });
                this.projectsMenu.attachView(sidebarProjectCollectionView);

                sidebarProjectCollectionView.render();

                var that = this;
                $(this.el).find('a.user-stream').on('click', function (e) {
                    e.preventDefault();
                    app.groupUserRouter.navigate($(this).attr('href'));
                    app.vent.trigger('home:show');
                    return false;
                });
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
            if (this.authenticatedUser) {
                // Render the layout and get it on the screen, first
                var sidebarLayoutView = new SidebarLayoutView({ model: this.authenticatedUser });

                sidebarLayoutView.on('show', function () {
                    app.vent.trigger('sidebar:rendered');
                });

                app.sidebar.show(sidebarLayoutView);
            }
        });

        return SidebarLayoutView;

    });