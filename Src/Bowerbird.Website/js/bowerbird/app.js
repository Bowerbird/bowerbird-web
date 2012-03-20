
window.Bowerbird.App = Backbone.Model.extend({
    defaults: {
        newObservation: null,
        clientId: null
    },

    initialize: function (options) {
        console.log('App.Initialize');
        this.teams = new Bowerbird.Collections.Teams();
        this.projects = new Bowerbird.Collections.Teams();
        this.stream = new Bowerbird.Models.Stream();
        this.chats = new Bowerbird.Collections.Chats();
        this.users = new Bowerbird.Collections.Users();
        this.activityRouter = new Bowerbird.ActivityRouter({ appManager: this, userId: this.get('userId')});
        this.chatRouter = new Bowerbird.ChatRouter({ appManager: this });
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
    }

});