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
        this.hub = options.groupHub;

        this.hub.newActivity = newActivity;
    };

    var ActivityController = {};

    var newActivity = function (data) {
        var activity = new Activity(data);

        app.vent.trigger('newactivity', activity);
        app.vent.trigger('newactivity:' + activity.get('Type'), activity);
        // Fire an event for each group the activity belongs to
        _.each(activity.get('Groups'), function (group) {
            this.vent.trigger('newactivity:' + group.Id, activity);
            this.vent.trigger('newactivity:' + group.Id + ':' + activity.get('Type'), activity);
        }, app);
    };

    app.addInitializer(function () {
        this.activityRouter = new ActivityRouter({
            groupHub: $.connection.groupHub
        });
    });

    return ActivityController;

});
