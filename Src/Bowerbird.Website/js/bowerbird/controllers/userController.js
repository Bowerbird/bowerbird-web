/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// UserController & UserRouter
// ---------------------------

define(['jquery', 'underscore', 'backbone', 'app'],
function ($, _, Backbone, app) {

    var UserRouter = function (options) {
        this.hub = $.connection.userHub;
        this.controller = options.controller;

        this.hub.setupOnlineUsers = setupOnlineUsers;
        this.hub.userStatusUpdate = userStatusUpdate;
        this.hub.joinedGroup = joinedGroup;
    };

    var UserController = {};

    // Hub Callbacks
    // -------------

    // Receive a usee status update
    var userStatusUpdate = function (userStatus) {
        log('activityController.userStatusUpdate', this, userStatus);

        app.onlineUsers.add(userStatus.User);

        // Then set their status
        app.onlineUsers.get(userStatus.User.Id).set('Status', 'Online');
    };

    // Receive a list of users that are online
    var setupOnlineUsers = function (onlineUsers) {
        log('activityController.setupOnlineUsers', this, onlineUsers);
        app.onlineUsers.add(onlineUsers);
    };

    var joinedGroup = function (group) {
        if (group.GroupType === 'project') {
            app.authenticatedUser.projects.add(group);
        }
        if (group.GroupType === 'team') {
            app.authenticatedUser.teams.add(group);
        }

        if (group.GroupType === 'organisation') {
            app.authenticatedUser.organisations.add(group);
        }
    };

    app.addInitializer(function () {
        var routes = {
            'users/:id/update': 'showUserForm'
        };

        this.userRouter = new UserRouter({
            controller: UserController,
            appRoutes: routes
        });
    });

    return UserController;

});
