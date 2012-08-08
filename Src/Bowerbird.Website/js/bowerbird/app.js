/// <reference path="../libs/log.js" />
/// <reference path="../libs/require/require.js" />
/// <reference path="../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../libs/underscore/underscore.js" />
/// <reference path="../libs/backbone/backbone.js" />
/// <reference path="../libs/backbone.marionette/backbone.marionette.js" />
/// <reference path="../libs/jquery.signalr/jquery-1.6.2-vsdoc.js" />

// Bowerbird.app
// -------------

// Initialises the app, but does not start rendering. That is done 
// when app.start() is called
define(['jquery', 'underscore', 'backbone', 'ich', 'bootstrap-data', 'models/user', 'collections/usercollection', 'collections/projectcollection', 'collections/teamcollection', 'collections/organisationcollection', 'collections/activitycollection', 'collections/exploreprojectcollection', 'collections/chatcollection', 'marionette', 'signalr'],
function ($, _, Backbone, ich, bootstrapData, User, UserCollection, ProjectCollection, TeamCollection, OrganisationCollection, ActivityCollection, ExploreProjectCollection, ChatCollection) {
    // Create an instance of the app
    var app = new Backbone.Marionette.Application();

    // Let's pollute the global namespace, just a little, for debug purposes :)
    window.Bowerbird = window.Bowerbird || {};
    window.Bowerbird.app = app;

    var AuthenticatedUser = function (data) {
        this.user = new User(data.User);
        this.memberships = data.Memberships;
        this.projects = new ProjectCollection(data.Projects);
        this.teams = new TeamCollection(data.Teams);
        this.organisations = new OrganisationCollection(data.Organisations);
        this.appRoot = data.AppRoot;

        this.hasGroupPermission = function (groupId, permissionId) {
            var membership = _.find(this.memberships, function (m) {
                return m.GroupId === groupId;
            });
            if (!membership) {
                return false;
            }
            return _.any(membership.PermissionIds, function (p) {
                return p === permissionId;
            });
        };

        this.defaultLicence = data.DefaultLicence;
    };

    app.vent.on('newactivity:groupadded', function (activity) {
        var group = activity.get('GroupAdded').Group;
        if (group.GroupType === 'project') {
            app.vent.trigger('projectAdded:', group);
            if (group.User.Id == app.authenticatedUser.user.id) {
                app.authenticatedUser.projects.add(group);
            }
        }
        if (group.GroupType === 'team') {
            app.vent.trigger('teamAdded:', group);
            if (group.User.Id == app.authenticatedUser.user.id) {
                app.authenticatedUser.teams.add(group);
            }
        }
        if (group.GroupType === 'organisation') {
            app.vent.trigger('organisationAdded:', group);
            if (group.User.Id == app.authenticatedUser.user.id) {
                app.authenticatedUser.organisations.add(group);
            }
        }

    }, this);

    app.addRegions({
        header: 'header',
        footer: 'footer',
        sidebar: '#sidebar',
        content: '#content',
        notifications: '#notifications',
        usersonline: '#onlineusers'
    });

    app.isPrerendering = function (name) {
        return name === app.prerenderedView.name && !app.prerenderedView.isBound;
    };

    app.setPrerenderComplete = function () {
        app.prerenderedView.isBound = true;
    };

    app.getShowViewMethodName = function (name) {
        if (!name) {
            var err = new Error("A name must be provided!");
            err.name = "BowerbirdNoViewNameProvidedError";
            throw err;
        }
        return app.isPrerendering(name) ? 'attachView' : 'show';
    };

    app.updateTitle = function (titleSegment) {
        var newTitle = 'Bowerbird';
        if (titleSegment.length > 0) {
            newTitle = titleSegment + ' - ' + newTitle;
        }
        document.title = newTitle;
    };

    app.routeHistory = [];
    app.previousContent = null;

    app.showFormContentView = function (view, name) {
        if (app.content.currentView) {
            // Store previous view in cache
            app.previousContent = {
                view: app.content.currentView,
                $el: app.content.currentView.$el.detach()
            };

            // Clear out current view
            app.content.currentView = null;
        }

        // Show new view
        app.content[app.getShowViewMethodName(name)](view);
    };

    app.showPreviousContentView = function () {
        if (app.previousContent) {
            // Close current view
            app.content.close();

            // Reinstate previous view
            app.content.$el.append(app.previousContent.$el);
            app.content.currentView = app.previousContent.view;
            app.routeHistory.shift();
            Backbone.history.navigate(_.first(app.routeHistory));

            // Clear out previous view cache
            app.previousContent = null;
        } else {
            // If we don't have a previous view, take user back to home stream
            Backbone.history.navigate('', { trigger: true });
        }
    };

    app.generateGuid = function () {
        var S4 = function () {
            return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
        };
        return (S4() + S4() + "-" + S4() + "-" + S4() + "-" + S4() + "-" + S4() + S4() + S4());
    };

    // Load the bootstrapped data into place
    app.bind('initialize:before', function () {
        // Override the marionette renderer so that it uses mustache templates 
        // together with icanhaz caching
        Backbone.Marionette.Renderer.render = function (template, data) {
            if (template) { // Marionette seems to call this method even if a view is created with a pre-existing DOM element. May need to investigate further.
                return ich[template](data);
            }
        };

        // Add additional capability if authenticated user
        if (bootstrapData.AuthenticatedUser) {
            // Add the authenticated user to the app for future reference
            app.authenticatedUser = new AuthenticatedUser(bootstrapData.AuthenticatedUser);

            // Online users
            app.onlineUsers = new UserCollection();

            // Chats
            app.chats = new ChatCollection();
            app.chatRegions = [];
        }

        // Add the prerendered view string to the app for use by controller duing init of first view
        app.prerenderedView = {
            name: bootstrapData.PrerenderedView,
            isBound: false, // Flag used to determine if prerenderd view has been bound to the object/DOM model
            data: bootstrapData.Model
        };
    });

    app.bind('initialize:after', function () {
        // Tasks to perform on DOM ready
        $(function () {
            // Only start history once app is fully initialised
            if (Backbone.history) {
                Backbone.history.on('route', function (route, name) {
                    app.routeHistory.unshift(Backbone.history.fragment);
                });

                // Start URL and history routing
                Backbone.history.start({ pushState: true });
            }

            // initialise the hub connection
            $.connection.hub.start({ transport: ['webSockets', 'longPolling'] }, function () {

                // Keep the client id
                app.clientId = $.signalR.hub.id;
                log('browser connected via signalr as ' + app.clientId);

                // Subscribe authenticated user to all their groups
                if (app.authenticatedUser) {
                    $.connection.userHub.registerUserClient(app.authenticatedUser.user.id)
                        .done(function () {
                            log('Added user to hub');
                        })
                        .fail(function (e) {
                            log('could not register client with hub', e);
                        });
                }
            });

            // Register closing of all popup menus in entire page
            $("body").click(function () {
                $('.sub-menu-button').removeClass('active'); // Make sure to add any new menu button types to the selector
            });
        });
    });

    return app;

});