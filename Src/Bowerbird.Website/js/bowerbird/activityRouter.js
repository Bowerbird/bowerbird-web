
window.Bowerbird.ActivityRouter = Backbone.Model.extend({

    // INIT-----------------------------------------

    initialize: function (options) {

        log('ActivityRouter.Initialize');
        _.bindAll(this, 'initHubConnection');

        this.appManager = options.appManager;
        this.activityHub = $.connection.activityHub;
        this.activityHub.userStatusUpdate = this.userStatusUpdate;
        this.activityHub.activityOccurred = this.activityOccurred;

        this.initHubConnection(options.userId);
        log('ActivityRouter.Initialize');
    },


    // TO HUB---------------------------------------

    initHubConnection: function (userId) {
        log('App.initHubConnection');
        var self = this;
        $.connection.hub.start({ transport: 'longPolling' },function () {
            self.activityHub.registerUserClient(userId)
                    .done(function () {
                        self.appManager.set('clientId', $.signalR.hub.id);
                        log('connected as ' + self.appManager.get('userId') + ' with ' + self.appManager.get('clientId'));
                    })
                    .fail(function (e) {
                        console.log(e);
                    });
        });
    },

    userStatusUpdate: function (data) {
        
        var user = app.users.get(data.id);
        if (_.isNull(user) || _.isUndefined(user)) {
            if (data.status == 2 || data.status == 3 || data.status == 'undefined') return;
            user = new Bowerbird.Models.User(data);
            app.users.add(user);
            log('app.userStatusUpdate: ' + data.name + ' logged in');
        } else {
            if (data.status == 2 || data.status == 3) {
                app.users.remove(user);
                log('app.userStatusUpdate: ' + data.name + ' logged out');
            }
            else {
                user.set('status', data.status);
                log('app.userStatusUpdate: ' + data.name + ' udpated their status');
            }
        }
    },


    // FROM HUB-------------------------------------

    activityOccurred: function (data) {
        // fire appropriate activity stream method..
    }

});
