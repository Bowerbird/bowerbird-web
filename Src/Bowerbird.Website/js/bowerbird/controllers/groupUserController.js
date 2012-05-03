/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// GroupUserController
// -------------------

// This is the app controller or sub-application for groups/users. It contains all of the 
// high level knowledge of how to run the app when it's in group/user mode.
define(['jquery', 'underscore', 'backbone', 'app', 'views/projectlayout', 'models/project', 'collections/streamitemcollection'], function ($, _, Backbone, app, ProjectLayout, Project, StreamItemCollection) {

    var GroupUserController = {};

    // Helper method to load project layout, taking into account bootstrapped data and prerendered view
    var showProjectLayout = function (id) {
        app.vent.trigger('project:show');
        var projectLayout = null;
        if (app.prerenderedView.name === 'projects' && !app.prerenderedView.isBound) {
            projectLayout = new ProjectLayout({ model: new Project(app.prerenderedView.data.Project) });
            app.content.attachView(projectLayout);
            app.prerenderedView.isBound = true;
        } else {
            projectLayout = new ProjectLayout(); // TODO: Get project using id...
            app.content.show(projectLayout);
        }
        projectLayout.render();
        return projectLayout;
    };

    // GroupUserController Public API
    // ------------------------------

    // Show the user's home page (i.e.: the "home stream")
    GroupUserController.showHome = function () {
        app.vent.trigger('home:show');
    };

    // Show a team
    GroupUserController.showTeam = function () {
        app.vent.trigger('team:show');
    };

    // Show project activity
    GroupUserController.showProjectStream = function (id) {
        var projectLayout = showProjectLayout(id);
        projectLayout.showStream(new StreamItemCollection(app.prerenderedView.data.StreamItems));
    };

    // Show project about
    GroupUserController.showProjectAbout = function (id) {
        var projectLayout = showProjectLayout(id);
        //projectLayout.details.showAbout();
    };

    // Show project members
    GroupUserController.showProjectMembers = function (id) {
        var projectLayout = showProjectLayout(id);
        //projectLayout.details.showMembers();
    };

    // Show a user
    GroupUserController.showUser = function () {
        app.vent.trigger('user:show');
    };

    //    // Show a list of email for the given category.
    //    MailApp.showCategory = function (category) {
    //        showFilteredEmailList(category);
    //        MailApp.Categories.showCategoryList();
    //    };

    //    // Show an individual email message, by Id
    //    MailApp.showMessage = function (messageId) {
    //        MailApp.emailList.onReset(function (list) {
    //            var email = list.get(messageId);
    //            MailApp.MailBox.showMessage(email);
    //        });
    //        MailApp.Categories.showCategoryList();
    //    };

    // GroupUserController Event Handlers
    // ----------------------------------

    //    // When a category is selected, filter the mail list
    //    // based on it.
    //    BBCloneMail.vent.bind("mail:category:show", function (category) {
    //        showFilteredEmailList(category);
    //    });

    //    // When the mail app is shown or `inbox` is clicked,
    //    // show all the mail.
    //    BBCloneMail.vent.bind("mail:show", function () {
    //        showFilteredEmailList();
    //    });

    // GroupUserController Initializer
    // -------------------------------

    // Initializes the email collection object with the list
    // of emails that are passed in from the call to 
    // `BBCloneMail.start`.
    //    app.addInitializer(function (options) {
    //        app.groupUserController = new GroupUserController();
    //        //MailApp.emailList.fetch();
    //    });

    return GroupUserController;

});
