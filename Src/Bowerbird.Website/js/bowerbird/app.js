
window.Bowerbird.App = Backbone.Model.extend({
    defaults: {
        newObservation: null,
        newProject: null,
        newOrganisation: null,
        newTeam: null,
        clientId: null
    },

    initialize: function (options) {
        log('App.Initialize');

        this.teams = new Bowerbird.Collections.Teams();
        this.projects = new Bowerbird.Collections.Projects();
        this.stream = new Bowerbird.Models.Stream();
        this.chats = new Bowerbird.Collections.Chats();
        this.users = new Bowerbird.Collections.Users();

        window.app = this;
        log('App.Initialize Completed');
    },

    start: function (userId, teams, projects, users) {
        // Start app page
        this.appView = new Bowerbird.Views.AppView({ app: this }).render();

        // Init sub components
        this.activityRouter = new Bowerbird.ActivityRouter({ userId: userId });
        this.chatRouter = new Bowerbird.ChatRouter();
        this.appRouter = new Bowerbird.AppRouter();
        
        // Populate with bootstrapped data
        this.teams.reset(teams);
        this.projects.reset(projects);
        this.users.reset(users);

        // Start URL and history routing
        Backbone.history.start({ pushState: false });
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
        this.set('newObservation', new Bowerbird.Models.Observation({ observedOn: new Date().format('d MMM yyyy') }));
    },

    cancelNewObservation: function () {
        this.set('newObservation', null);
    },

    startNewProject: function () {
        this.set('newProject', new Bowerbird.Models.Project({}));
    },

    cancelNewProject: function () {
        this.set('newProject', null);
    },

    startNewOrganisation: function () {
        this.set('newOrganisation', new Bowerbird.Models.Organisation({}));
    },

    cancelNewOrganisation: function () {
        this.set('newOrganisation', null);
    },

    startNewTeam: function () {
        this.set('newTeam', new Bowerbird.Models.Team({}));
    },

    cancelNewTeam: function () {
        this.set('newTeam', null);
    }

});
