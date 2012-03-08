
window.Bowerbird.App = Backbone.Model.extend({
    defaults: {
        newObservation: null,
        clientId: null
    },

    initialize: function (options) {
        this.teams = new Bowerbird.Collections.Teams();
        this.projects = new Bowerbird.Collections.Teams();
        this.stream = new Bowerbird.Models.Stream();
        this.chats = new Bowerbird.Collections.Chats();
        this.chatManager = new Bowerbird.ChatManager({ appManager: this });
        this.initHubConnection(options.userId);
    },

    showHomeStream: function (filter) {
        this.stream.setNewStream(null, filter);
    },

    showTeamStream: function (id, filter) {
        this.stream.setNewStream(this.teams.get('teams/' + id), filter);
    },

    showProjectStream: function (id, filter) {
        this.stream.setNewStream(this.projects.get('projects/' + id), filter);
    },

    showUserStream: function (user, filter) {

    },

    startNewObservation: function () {
        this.set('newObservation', new Bowerbird.Models.Observation());
    },

    cancelNewObservation: function () {
        this.set('newObservation', null);
    },

    initHubConnection: function (userId) {
        console.log('app.initHubConnection');
        var self = this;
        var activityHub = $.connection.activityHub;
        $.connection.hub.start(function () {
            activityHub.registerClientUser(userId)
                    .done(function () {
                        self.set('clientId', $.signalR.hub.id);
                        console.log('connected as ' + self.get('userId') + ' with ' + self.get('clientId'));
                        //startServerConnectionRefreshing();
                        //window.activityHub.getCurrentlyConnectedUsers();
                    })
                    .fail(function (e) {
                        console.log(e);
                    });
        });
    }
});

