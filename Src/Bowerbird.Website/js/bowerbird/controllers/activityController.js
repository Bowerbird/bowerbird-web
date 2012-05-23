/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ActivityController
// ------------------

define(['jquery', 'underscore', 'backbone', 'app', 'signalr'], function ($, _, Backbone, app) {

    var ActivityController = {};



//    var init = function (options) {
//        this.notificationHub = $.connection.notificationHub;
//        this.notificationHub.userStatusUpdate = this.userStatusUpdate;

//        this.notificationHub.newNotification = this.newNotification;
//        this.notificationHub.newStreamItem = this.newStreamItem;

//        this.initHubConnection(options.userId);
//        log('ActivityRouter.Initialize');
//    };

//        // TO HUB---------------------------------------

//        initHubConnection: function (userId) {
//            log('App.initHubConnection');
//            var self = this;
//            $.connection.hub.start({ transport: 'longPolling' }, function () {
//                self.notificationHub.registerUserClient(userId)
//                    .done(function () {
//                        app.set('clientId', $.signalR.hub.id);
//                        log('connected as ' + userId + ' with ' + app.get('clientId'));
//                    })
//                    .fail(function (e) {
//                        log(e);
//                    });
//            });
//        },

//        userStatusUpdate: function (data) {
//            app.onlineUsers.updateUserStatus(data);
//        },

//        // FROM HUB-------------------------------------

//        newNotification: function (notification) {
//            app.notifications.add(notification);
//        },

//        newStreamItem: function (streamItem) {
//            app.stream.addStreamItem(streamItem);
//        }
//    });

    return ActivityController;

});
