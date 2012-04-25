/// <reference path="../libs/log.js" />
/// <reference path="../libs/jquery-1.7.1.min.js" />
/// <reference path="../libs/underscore.js" />
/// <reference path="../libs/backbone.js" />
/// <reference path="../libs/backbone.marionette.js" />

// GroupUserController
// -------------------

// This is the app controller or sub-application for groups/users. It contains all of the 
// high level knowledge of how to run the app when it's in group/user mode.
Bowerbird.Controllers.GroupUserController = (function (app, Bowerbird, Backbone, $, _) {

    var GroupUserController = {};

    // GroupUserController Helper Methods
    // ----------------------------------

    // Filter the mail by the category, if one was specified
    //    var showFilteredEmailList = function (category) {
    //        MailApp.emailList.onReset(function (list) {
    //            var filteredMail = list.forCategory(category);
    //            MailApp.MailBox.showMail(filteredMail);
    //        });
    //    }

    // GroupUserController Public API
    // ------------------------------

    // Show the user's home page (i.e.: the "home stream")
    GroupUserController.showHome = function () {
        app.vent.trigger('home:show');
        alert('home');
    };

    // Show a team
    GroupUserController.showTeam = function () {
        app.vent.trigger('team:show');
    };

    // Show a project
    GroupUserController.showProject = function () {
        app.vent.trigger('project:show');
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
    //    app.addInitializer(function () {
    //        Bowerbird.app.groupUserController = new MailApp.EmailCollection();
    //        MailApp.emailList.fetch();
    //    });

    return GroupUserController;

})(Bowerbird.app, Bowerbird, Backbone, jQuery, _);
