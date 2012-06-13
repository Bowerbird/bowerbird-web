/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ActivityRouter
// --------------

//define(['jquery', 'underscore', 'backbone', 'app', 'controllers/activitycontroller'], function ($, _, Backbone, app, ActivityController) {

//    var ActivityRouter = function (options) {
//        this.hub = $.connection.activityHub;
//        this.controller = options.controller;
//        this.hub.newActivity = this.controller.newActivity;
//        this.hub.userStatusUpdate = this.controller.userStatusUpdate;

////        var self = this;
////        var method = _.bind(this.controller.userStatusUpdate, this.controller);
////        this.hub.userStatusUpdate = function (result) {
////            method.call(self.controller, result);
////        };
//    };

//    app.addInitializer(function () {
//        this.activityRouter = new ActivityRouter({
//            controller: ActivityController
//        });
//    });

//    return ActivityRouter;

//});