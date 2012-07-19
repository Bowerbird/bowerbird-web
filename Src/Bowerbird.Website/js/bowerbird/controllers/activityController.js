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

    var newActivity = function (groupId, activityData) {
        var activity = new Activity(activityData);

        app.vent.trigger('newactivity', activity);
        app.vent.trigger('newactivity:' + activity.get('Type'), activity);
        app.vent.trigger('newactivity:' + groupId, activity);
        app.vent.trigger('newactivity:' + groupId + ':' + activity.get('Type'), activity);

        if (activityData.ContributionId) {
            app.vent.trigger('newactivity:' + activity.get('Type') + ':' + activity.get('ContributionId'), activity);
            log('triggered: ' + 'newactivity:' + activity.get('Type') + ':' + activity.get('ContributionId'), activity);
        }
    };

    app.addInitializer(function () {
        this.activityRouter = new ActivityRouter({
            groupHub: $.connection.groupHub
        });
    });

    return ActivityController;

});
