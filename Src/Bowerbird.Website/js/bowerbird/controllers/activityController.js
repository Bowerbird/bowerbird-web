/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ActivityController
// ------------------

define(['jquery', 'underscore', 'backbone', 'app', 'models/activity'], function ($, _, Backbone, app, Activity) {

    var ActivityController = {};

    ActivityController.newActivity = function (data) {
        log('activityController.newActivity', this, data);
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

    return ActivityController;

});