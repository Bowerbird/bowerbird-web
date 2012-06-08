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
'signalr',
'bootstrap-data',
'models/user',
'collections/usercollection',
'collections/projectcollection',
'collections/teamcollection',
'collections/organisationcollection',
'collections/activitycollection',
'marionette'
],
function ($, _, Backbone, signalr, bootstrapData, User, UserCollection, ProjectCollection, TeamCollection, OrganisationCollection, ActivityCollection) {

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
            log('ssssssssssssssssssssssssssss');
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
        usersonline: '#onlineusers'
    });

    // Load the bootstrapped data into place
    app.bind('initialize:before', function () {
        // Online users
        app.onlineUsers = new UserCollection();

        // Add the authenticated user to the app for future reference
        if (bootstrapData.AuthenticatedUser) {
            app.authenticatedUser = new AuthenticatedUser(bootstrapData.AuthenticatedUser);
        }

        if (bootstrapData.OnlineUsers) {
            app.onlineUsers.add(bootstrapData.OnlineUsers);
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

    // Only start history once app is fully initialised
    app.bind('initialize:after', function () {
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

    // On DOM ready tasks
    $(function () {
        // Start the app as soon as the DOM is ready, loading in the bootstrapped data
        app.start(bootstrapData);


        //        // Register overriding all anchors to channel through router
        //        $('a').on('click', function (e) {

        //            if (app.groupUserRouter.navigate($(this).attr('href'), { trigger: true })) {
        //                e.preventDefault();
        //                log('caught by groupUserRouter');
        //                return false;
        //            }
        //            if (app.contributionRouter.navigate($(this).attr('href'), { trigger: true })) {
        //                e.preventDefault();
        //                log('caught by contributionRouter');
        //                return false;
        //            }
        //            log('not caught!');
        //            //app.contributionRouter.navigate($(this).attr('href'), { trigger: true })
        //            return true;
        //        });

        // Register closing of all popup menus in entire page
        $("body").click(function () {
            $('.sub-menu-button').removeClass('active'); // Make sure to add any new menu button types to the selector
        });
    });

    return app;

});
