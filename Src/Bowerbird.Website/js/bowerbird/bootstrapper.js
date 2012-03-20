
function startBowerbird(teams, projects, users, userId) {
    window.app = new Bowerbird.App({ userId: userId });
    console.log('bootstrapper - app created');
    app.appView = new Bowerbird.Views.AppView();
    app.appView.showStreamView();
    console.log('bootstrapper - appView created');
    app.router = new Bowerbird.Router();
    Backbone.history.start({ pushState: false });
    app.teams.add(teams);
    app.projects.add(projects);
    app.users.reset(users);
    console.log('bootstrapper - initialized');
}