
window.Bowerbird.ActivityRouter = Backbone.Model.extend({

    // INIT-----------------------------------------

    initialize: function (options) {

        console.log('ActivityRouter.Initialize');
        _.bindAll(this, 'initHubConnection');

        this.appManager = options.appManager;
        this.activityHub = $.connection.activityHub;
        this.activityHub.userStatusUpdate = this.userStatusUpdate;
        this.activityHub.activityOccurred = this.activityOccurred;

        this.initHubConnection(options.userId);
        console.log('ActivityRouter.Initialize');
    },


    // TO HUB---------------------------------------

    initHubConnection: function (userId) {
        console.log('App.initHubConnection');
        var self = this;
        $.connection.hub.start({ transport: 'longPolling' },function () {
            self.activityHub.registerUserClient(userId)
                    .done(function () {
                        self.appManager.set('clientId', $.signalR.hub.id);
                        console.log('connected as ' + self.appManager.get('userId') + ' with ' + self.appManager.get('clientId'));
                    })
                    .fail(function (e) {
                        console.log(e);
                    });
        });
    },

    userStatusUpdate: function (data) {
        console.log('app.userStatusUpdate');
        var user = app.users.get(data.id);
        if (_.isNull(user) || _.isUndefined(user)) {
            if (data.status == 2 || data.status == 3 || data.status == 'undefined') return;
            user = new Bowerbird.Models.User(data);
            app.users.add(user);
        } else {
            if (data.status == 2 || data.status == 3) {
                user.remove();
            }
            else {
                user.set('status', data.status);
            }
        }
    },


    // FROM HUB-------------------------------------

    activityOccurred: function (data) {
        // fire appropriate activity stream method..
    }

});
