/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// UserController & UserRouter
// ---------------------------

define(['jquery', 'underscore', 'backbone', 'app', 'models/user'],
function ($, _, Backbone, app, User) {
    var UserRouter = Backbone.Marionette.AppRouter.extend({
        appRoutes: {
            'users/:id': 'showUserDetails'
        }
    });

    var UserHubRouter = function (options) {
        this.userHub = options.hub;
        this.userHub.setupOnlineUsers = setupOnlineUsers;
        this.userHub.userStatusUpdate = userStatusUpdate;
        this.userHub.joinedGroup = joinedGroup;
        this.userHub.mediaResourceUploadSuccess = mediaResourceUploadSuccess;
        this.userHub.mediaResourceUploadFailure = mediaResourceUploadFailure;

        // ping the server with the user's latest activity
        this.updateUserClientStatus = function (userId, latestHeartbeat, latestInteractivity) {
            log(userId, latestHeartbeat, latestInteractivity);
            $.connection.userHub.updateUserClientStatus(userId, latestHeartbeat, latestInteractivity);
        };
    };

    var UserController = {};

    var getModel = function (id) {
        var url = '/users/create';
        if (id) {
            url = id;
        }
        var deferred = new $.Deferred();
        if (app.isPrerenderingView('users')) {
            deferred.resolve(app.prerenderedView.data);
        } else {
            $.ajax({
                url: url
            }).done(function (data) {
                deferred.resolve(data.Model);
            });
        }
        return deferred.promise();
    };

    // Hub Callbacks
    // -------------

    // Receive a list of users that are online
    var setupOnlineUsers = function (onlineUsers) {
        log('userController.setupOnlineUsers', this, onlineUsers);
        app.onlineUsers.add(onlineUsers);
    };

    // Receive a user status update
    var userStatusUpdate = function (userStatus) {
        log('userController.userStatusUpdate', this, userStatus);

        if (!app.onlineUsers.get(userStatus.User.Id)) {
            app.onlineUsers.add(userStatus.User);
        }

        // Then set their status
        app.onlineUsers.get(userStatus.User.Id).set('LatestActivity', userStatus.User.LatestActivity);
        app.onlineUsers.get(userStatus.User.Id).set('LatestHeartbeat', userStatus.User.LatestHeartbeat);
    };

    // Receive a user joined group update
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

    // Receive a media upload success notification
    var mediaResourceUploadSuccess = function (mediaResource) {
        app.vent.trigger('mediaresourceuploadsuccess', mediaResource);
    };

    // Receive a media upload failure notification
    var mediaResourceUploadFailure = function (key, reason) {
        app.vent.trigger('mediaresourceuploadfailure', key, reason);
    };

    UserController.showUserDetails = function (id) {
    };

    app.addInitializer(function () {
        log('userController.initialize');

        this.userRouter = new UserRouter({
            controller: UserController
        });
        this.userHubRouter = new UserHubRouter({
            hub: $.connection.userHub
        });
    });

    return UserController;
});