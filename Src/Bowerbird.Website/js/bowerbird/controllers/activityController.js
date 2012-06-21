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
        this.hub = $.connection.groupHub;
        this.controller = options.controller;

        this.hub.newActivity = this.controller.newActivity;
        //this.hub.userStatusUpdate = this.controller.userStatusUpdate;
        //this.hub.setupOnlineUsers = this.controller.setupOnlineUsers;

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

//    ActivityController.userStatusUpdate = function (data) {
//        log('activityController.userStatusUpdate', this, data);

//        // If user doesn't exist, add them
//        app.onlineUsers.add(data);

//        // Then set their status
//        app.onlineUsers.get(data.Id).set('Status', data.Status);
//    };

//    ActivityController.setupOnlineUsers = function (data) {
//        log('activityController.setupOnlineUsers', this, data);
//        app.onlineUsers.add(data);
//    };

    app.addInitializer(function () {
        this.activityRouter = new ActivityRouter({
            controller: ActivityController
        });
    });

    return ActivityController;

});
