
window.Bowerbird.NotificationRouter = Backbone.Model.extend({
    initialize: function (options) {

        log('ActivityRouter.Initialize');
        _.bindAll(this, 'initHubConnection');

        this.notificationHub = $.connection.notificationHub;
        this.notificationHub.userStatusUpdate = this.userStatusUpdate;

        this.notificationHub.newNotification = this.newNotification;
        this.notificationHub.newStreamItem = this.newStreamItem;

        this.initHubConnection(options.userId);
        log('ActivityRouter.Initialize');
    },

    // TO HUB---------------------------------------

    initHubConnection: function (userId) {
        log('App.initHubConnection');
        var self = this;
        $.connection.hub.start({ transport: 'longPolling' }, function () {
            self.notificationHub.registerUserClient(userId)
                    .done(function () {
                        app.set('clientId', $.signalR.hub.id);
                        log('connected as ' + userId + ' with ' + app.get('clientId'));
                    })
                    .fail(function (e) {
                        log(e);
                    });
        });
    },

    userStatusUpdate: function (data) {
        app.onlineUsers.updateUserStatus(data);
    },

    // FROM HUB-------------------------------------

    newNotification: function (notification) {
        app.notifications.add(notification);
    },

    newStreamItem: function (streamItem) {
        app.stream.addStreamItem(streamItem);
    }
});
