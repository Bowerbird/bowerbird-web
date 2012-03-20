// usage: log('inside coolFunc', this, arguments);
// paulirish.com/2009/log-a-lightweight-wrapper-for-consolelog/
window.log = function () {
    log.history = log.history || [];   // store logs to an array for reference
    log.history.push(arguments);
    if (this.console) console.log(Array.prototype.slice.call(arguments));
};

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