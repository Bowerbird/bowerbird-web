/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ActivityRouter
// --------------

define(['jquery', 'underscore', 'backbone', 'app', 'controllers/activitycontroller', 'signalr'], function ($, _, Backbone, app, ActivityController) {

    var ActivityRouter = function (options) {
        this.hub = $.connection.activityHub;
        this.controller = options.controller;

        this.hub.newActivity = this.controller.newActivity;
    };

    //    _.extend(ActivityRouter.prototype, Events, {

    //    });

    app.addInitializer(function () {
        this.activityRouter = new ActivityRouter({
            controller: ActivityController
        });

        $.connection.hub.start({ transport: 'longPolling' }, function () {
            this.activityRouter.hub.registerUserClient(userId)
                    .done(function () {
                        //app.set('clientId', $.signalR.hub.id);
                        log('connected as ' + userId + ' with ' + app.get('clientId'));
                    })
                    .fail(function (e) {
                        log(e);
                    });
        });
    });

    return ActivityRouter;

});
