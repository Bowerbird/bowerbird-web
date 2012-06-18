/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ActivityController & ActivityRouter
// -----------------------------------

define(['jquery', 'underscore', 'backbone', 'app', 'models/activity'],
function ($, _, Backbone, app, Activity) {

    var ActivityRouter = function (options) {
        this.hub = $.connection.activityHub;
        this.controller = options.controller;

        this.hub.newActivity = this.controller.newActivity;
        this.hub.userStatusUpdate = this.controller.userStatusUpdate;
        this.hub.setupOnlineUsers = this.controller.setupOnlineUsers;

        this.hub.debugToLog = this.controller.debugToLog;
    };

    var ActivityController = {};

    ActivityController.debugToLog = function (message) {
        log(message);
    }

    ActivityController.newActivity = function (data) {
        log('activityController.newActivity', this, data);
        // TODO: Maybe add another collection that only contains your membership activities for the notification list
        app.activities.add(data);
    };

    ActivityController.userStatusUpdate = function (data) {
        log('activityController.userStatusUpdate', this, data);

        if (!_.any(app.onlineUsers, function (user) { return user.id == data.Id; })) {
            if (data.Status == 2 || data.Status == 3 || data.Status == 'undefined') return;
            var user = new User(data);
            app.onlineUsers.add(user);
        } else {
            var user = app.onlineUsers.get(data.Id);
            if (data.Status == 2 || data.Status == 3) {
                app.onlineUsers.remove(user);
            } else {
                user.set('Status', data.Status);
            }
        }
    };

    ActivityController.setupOnlineUsers = function (data) {
        log('activityController.setupOnlineUsers', this, data);
        app.onlineUsers.add(data);
    };

    app.addInitializer(function () {
        this.activityRouter = new ActivityRouter({
            controller: ActivityController
        });
    });

    return ActivityController;

});
