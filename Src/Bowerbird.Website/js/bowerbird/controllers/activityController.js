/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ActivityController
// ------------------

define(['jquery', 'underscore', 'backbone', 'app', 'models/user'], function ($, _, Backbone, app, User) {

    var ActivityController = {};

    // Call From Hub
    ActivityController.newActivity = function (data) {
        log('activityController.newActivity');
        log(data);
        alert('new activity received');
    };

    // Call From Hub
    ActivityController.userStatusUpdate = function (data) {
        log('activityController.userStatusUpdate');
        log(data);
        //app.onlineUsers.updateUserStatus(data);

        if (!app.onlineUsers.contains(data.Id)) {
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

    return ActivityController;

});