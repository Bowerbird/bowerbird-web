
function startBowerbird(teams, projects, userId) {
    window.app = new Bowerbird.App({ userId: userId });
    app.appView = new Bowerbird.Views.AppView();
    app.appView.showStreamView();
    app.router = new Bowerbird.Router();
    Backbone.history.start({ pushState: false });
    app.teams.add(teams);
    app.projects.add(projects);
}