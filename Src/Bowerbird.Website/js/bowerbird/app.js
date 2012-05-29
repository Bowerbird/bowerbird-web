/// <reference path="../libs/log.js" />
/// <reference path="../libs/require/require.js" />
/// <reference path="../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../libs/underscore/underscore.js" />
/// <reference path="../libs/backbone/backbone.js" />
/// <reference path="../libs/backbone.marionette/backbone.marionette.js" />

// Bowerbird.app
// -------------

// Initialises the app, but does not start rendering. That is done 
// when app.start() is called
define(['jquery', 'underscore', 'backbone', 'bootstrap-data', 'models/user', 'collections/usercollection', 'collections/projectcollection', 'collections/teamcollection', 'collections/organisationcollection', 'marionette'],
function ($, _, Backbone, bootstrapData, User, UserCollection, ProjectCollection, TeamCollection, OrganisationCollection) {

    // Create an instance of the app
    var app = new Backbone.Marionette.Application();

    // Let's pollute the global namespace, just a little, for debug purposes :)
    window.Bowerbird = window.Bowerbird || {};
    window.Bowerbird.version = '1.0.0';
    window.Bowerbird.app = app;

    app.addRegions({
        header: 'header',
        footer: 'footer',
        sidebar: '#sidebar',
        content: '#content',
        notifications: '#notifications',
        onlineUsers: '#online-users'
    });

    // Load the bootstrapped data into place
    app.bind('initialize:before', function () {
        // Online users
        app.onlineUsers = new UserCollection();

        // Add the authenticated user to the app for future reference
        if (bootstrapData.AuthenticatedUser) {
            app.user = new User(bootstrapData.AuthenticatedUser.User);
            app.userProjects = new ProjectCollection(bootstrapData.AuthenticatedUser.Projects);
            app.teams = new TeamCollection(bootstrapData.AuthenticatedUser.Teams);
            app.organisations = new OrganisationCollection(bootstrapData.AuthenticatedUser.Organisations);
        }

        if (bootstrapData.OnlineUsers) {
            app.onlineUsers.add(bootstrapData.OnlineUsers);
        }

        // Add the prerendered view string to the app for use by controller duing init of first view
        app.prerenderedView = {
            name: bootstrapData.PrerenderedView,
            isBound: false, // Flag used to determine if prerenderd view has been bound to the object/DOM model
            data: bootstrapData.Model
        };
    });

    // Only start history once app is fully initialised
    app.bind('initialize:after', function () {
        if (Backbone.history) {
            // Start URL and history routing
            Backbone.history.start({ pushState: true });
        }
    });

    app.isPrerendering = function (name) {
        return name === app.prerenderedView.name && !app.prerenderedView.isBound;
    };

    app.setPrerenderComplete = function () {
        app.prerenderedView.isBound = true;
    };

    app.getShowViewMethodName = function (name) {
        return app.isPrerendering(name) ? 'attachView' : 'show';
    };

    // On DOM ready tasks
    $(function () {
        // Start the app as soon as the DOM is ready, loading in the bootstrapped data
        app.start(bootstrapData);

        //        // Register overriding all anchors to channel through router
        //        $('a').on('click', function (e) {

        //            if (app.groupUserRouter.navigate($(this).attr('href'), { trigger: true })) {
        //                e.preventDefault();
        //                log('caught by groupUserRouter');
        //                return false;
        //            }
        //            if (app.contributionRouter.navigate($(this).attr('href'), { trigger: true })) {
        //                e.preventDefault();
        //                log('caught by contributionRouter');
        //                return false;
        //            }
        //            log('not caught!');
        //            //app.contributionRouter.navigate($(this).attr('href'), { trigger: true })
        //            return true;
        //        });

        // Register closing of all popup menus in entire page
        $("body").click(function () {
            $('.sub-menu-button').removeClass('active'); // Make sure to add any new menu button types to the selector
        });
    });

    return app;

});
