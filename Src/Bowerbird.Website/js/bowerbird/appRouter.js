
window.Bowerbird.AppRouter = Backbone.Router.extend({
    routes: {
        '': 'showHomeStream',
        'teams/:id': 'showTeamStream',
        'projects/:id': 'showProjectStream',
        'users/:id': 'showUserStream',
        'observations/create': 'startNewObservation',
        'projects/create': 'startNewProject',
        'organisations/create': 'startNewOrganisation',
        'teams/create': 'startNewTeam',
        'explore/projects': 'exploreProjects',
        'explore/teams': 'exploreTeams',
        'explore/organisations': 'exploreOrganisations'
    },

    initialize: function (options) {
        _.extend(this, Backbone.Events);
    },

    showHomeStream: function () {
        app.showHomeStream('all');
    },

    showTeamStream: function (id) {
        app.showTeamStream(id, 'all');
    },

    showProjectStream: function (id) {
        app.showProjectStream(id, 'all');
    },

    showUserStream: function (id) {
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
    },

    exploreProjects: function () {
        app.exploreProjects();
    },

    exploreTeams: function () {
        app.exploreTeams();
    },

    exploreOrganisations: function () {
        app.exploreOrganisations();
    }
});