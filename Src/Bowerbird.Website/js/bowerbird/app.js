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
define([
    'jquery',
    'underscore',
    'backbone',
    'ich',
    'bootstrap-data',
    'models/user',
    'collections/usercollection',
    'collections/projectcollection',
    'collections/teamcollection',
    'collections/organisationcollection',
    'collections/activitycollection',
    'collections/exploreprojectcollection',
    'collections/chatcollection',
    'marionette',
    'signalr'],
    function (
        $,
        _,
        Backbone,
        ich,
        bootstrapData,
        User,
        UserCollection,
        ProjectCollection,
        TeamCollection,
        OrganisationCollection,
        ActivityCollection,
        ExploreProjectCollection,
        ChatCollection
        ) {

        // Create an instance of the app
        var app = new Backbone.Marionette.Application();

        // Let's pollute the global namespace, just a little, for debug purposes :)
        window.Bowerbird = window.Bowerbird || {};
        window.Bowerbird.version = '1.0.0';
        window.Bowerbird.app = app;

        var AuthenticatedUser = function (data) {
            this.user = new User(data.User);
            this.memberships = data.Memberships;
            this.projects = new ProjectCollection(data.Projects);
            this.teams = new TeamCollection(data.Teams);
            this.organisations = new OrganisationCollection(data.Organisations);
            this.appRoot = data.Application;

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

            app.vent.on('newactivity:groupadded', function (activity) {
                var group = activity.get('GroupAdded').Group;
                if (group.GroupType === 'project') {
                    app.authenticatedUser.projects.add(group);
                }
            }, this);
        };

        app.addRegions({
            header: 'header',
            footer: 'footer',
            sidebar: '#sidebar',
            content: '#content',
            notifications: '#notifications',
            usersonline: '#onlineusers',
            chatarea: '#chatarea'
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

        // Load the bootstrapped data into place
        app.bind('initialize:before', function () {
            // Override the marionette renderer so that it uses mustache templates 
            // together with icanhaz caching
            Backbone.Marionette.Renderer.render = function (template, data) {
                if (template) { // Marionette seems to call this method even if a view is created with a pre-existing DOM element. May need to investigate further.
                    return ich[template](data);
                }
            };

            // Online users
            app.onlineUsers = new UserCollection();

            //chats
            app.chats = new ChatCollection();

            // Add the authenticated user to the app for future reference
            if (bootstrapData.AuthenticatedUser) {
                app.authenticatedUser = new AuthenticatedUser(bootstrapData.AuthenticatedUser);
            }

            if (app.authenticatedUser) {
                if (bootstrapData.OnlineUsers) {
                    app.onlineUsers.add(bootstrapData.OnlineUsers);
                }
            }

            // Add the prerendered view string to the app for use by controller duing init of first view
            app.prerenderedView = {
                name: bootstrapData.PrerenderedView,
                isBound: false, // Flag used to determine if prerenderd view has been bound to the object/DOM model
                data: bootstrapData.Model
            };

            app.activities = new ActivityCollection();

            app.activities.on(
            'add',
            function (activity) {
                this.vent.trigger('newactivity', activity);
                this.vent.trigger('newactivity:' + activity.get('Type'), activity);
            },
            this);


            app.contentHistory = [];
        });

        app.bind('initialize:after', function () {
            // Tasks to perform on DOM ready
            $(function () {
                // Only start history once app is fully initialised
                if (Backbone.history) {
                    // Start URL and history routing
                    Backbone.history.start({ pushState: true });
                }

                // initialise the hub connection
                $.connection.hub.start({ transport: 'longPolling' }, function () {
                    $.connection.activityHub.registerUserClient(app.authenticatedUser.user.id)
                    .done(function () {
                        app.clientId = $.signalR.hub.id;
                        log('connected as ' + app.authenticatedUser.user.id + ' with ' + app.clientId);
                    })
                    .fail(function (e) {
                        log(e);
                    });
                });

                // Register closing of all popup menus in entire page
                $("body").click(function () {
                    $('.sub-menu-button').removeClass('active'); // Make sure to add any new menu button types to the selector
                });
            });

        });

        return app;

    });