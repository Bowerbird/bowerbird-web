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

    app.addInitializer(function () {
        this.userRouter = new UserRouter({
            controller: UserController
        });
    });

    return UserController;

});
