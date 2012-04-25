
window.Bowerbird.App = Backbone.Model.extend({
    defaults: {
        newObservation: null,
        newProject: null,
        newOrganisation: null,
        newTeam: null,
        clientId: null,
        authenticatedUser: null,
        prerenderedView: null
    },

    initialize: function (options) {
        log('App.Initialize');

        this.teams = new Bowerbird.Collections.Teams();
        this.projects = new Bowerbird.Collections.Projects();
        this.stream = new Bowerbird.Models.Stream();
        this.chats = new Bowerbird.Collections.Chats();
        this.onlineUsers = new Bowerbird.Collections.Users();
        this.notifications = new Bowerbird.Collections.Notifications();
        this.explore = new Bowerbird.Models.Explore();

        window.app = this;
        log('App.Initialize Completed');
    },

    start: function (authenticatedUser, teams, projects, users, prerenderedView) {
        // Start app page
        this.appView = new Bowerbird.Views.AppView().render();

        // Init sub components
        this.notificationRouter = new Bowerbird.NotificationRouter({ userId: authenticatedUser.Id });
        this.chatRouter = new Bowerbird.ChatRouter();
        this.appRouter = new Bowerbird.AppRouter();

        // Populate with bootstrapped data
        this.set('authenticatedUser', authenticatedUser);
        this.set('prerenderedView', prerenderedView);
        this.teams.reset(teams);
        this.projects.reset(projects);
        this.onlineUsers.reset(users);

        // Override all anchors to channel through router
        $('a').on('click', function (e) {
            e.preventDefault();
            app.appRouter.navigate($(this).attr('href'), true);
            return false;
        });

        // Start URL and history routing
        Backbone.history.start({ pushState: true });
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
        this.set('newObservation', new Bowerbird.Models.Observation({ ObservedOn: new Date().format('d MMM yyyy') }));
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
    },

    exploreProjects: function () {
        this.explore.setNewExplore('project');
    },

    exploreTeams: function () {
        this.explore.setNewExplore('team');
    },

    exploreOrganisations: function () {
        this.explore.setNewExplore('organisation');
    }

});
