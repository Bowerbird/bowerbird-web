
window.Bowerbird.AppRouter = Backbone.Router.extend({
    routes: {
        '': 'showDefaultStream',
        'home/:filter': 'showHomeStream',
        'teams/:id/:filter': 'showTeamStream',
        'projects/:id/:filter': 'showProjectStream',
        'users/:id/:filter': 'showUserStream',
        'observation/create': 'startNewObservation',
        'project/create': 'startNewProject',
        'organisation/create': 'startNewOrganisation',
        'team/create': 'startNewTeam'
    },

    initialize: function (options) {
        _.extend(this, Backbone.Events);
    },

    showDefaultStream: function () {
        app.showHomeStream('all');
    },

    showHomeStream: function (filter) {
        app.showHomeStream(filter);
    },

    showTeamStream: function (id, filter) {
        app.showTeamStream(id, filter);
    },

    showProjectStream: function (id, filter) {
        app.showProjectStream(id, filter);
    },

    showUserStream: function (id, filter) {

    },

    startNewObservation: function () {
        app.startNewObservation();
    },

    startNewProject: function () {
        app.startNewProject();
    },

    startNewOrganisation: function () {
        app.startNewOrganisation();
    },

    startNewTeam: function () {
        app.startNewTeam();
    }
});