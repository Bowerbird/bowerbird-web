/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// HomeController
// --------------
define(['jquery', 'underscore', 'backbone', 'app', 'views/homelayoutview'],
function ($, _, Backbone, app, HomeLayoutView) 
{
    var HomeController = {};

    // Public API
    // ----------

    HomeController.showHomeStream = function () {
        $(function () {
            log('showing home', this, this);

            var previousView = _.find(app.contentHistory, function (item) {
                return item.key == 'home';
            });

            if (!previousView) {
                homeLayoutView = new HomeLayoutView({ model: app.authenticatedUser.user });
                app.contentHistory.push({ key: 'home', view: homeLayoutView });
            } else {
                homeLayoutView = previousView.view;
            }

            app.content[app.getShowViewMethodName('home')](homeLayoutView);

            if (app.isPrerendering('home')) {
                homeLayoutView.showBootstrappedDetails();
            }

            homeLayoutView.showStream();

            app.setPrerenderComplete();
        });
    };

    return HomeController;

});

// HomeRouter
// ----------
define(['jquery', 'underscore', 'backbone', 'app', 'controllers/homecontroller'],
function ($, _, Backbone, app, HomeController) 
{
    var HomeRouter = Backbone.Marionette.AppRouter.extend({
        appRoutes: {
            '': 'showHomeStream'
        }
    });

    app.addInitializer(function () {
        this.homeRouter = new HomeRouter({
            controller: HomeController
        });
    });
});