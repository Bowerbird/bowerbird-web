/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// GroupUserController & GroupUserRouter
// -------------------------------------

// This is the controller for groups/users. It contains all of the 
// high level knowledge of how to run the app when it's in group/user mode.
define(['jquery', 'underscore', 'backbone', 'app', 'views/projectlayoutview', 'models/project', 'collections/streamitemcollection'],
function ($, _, Backbone, app, ProjectLayoutView, Project, StreamItemCollection) 
{
    var GroupUserRouter = Backbone.Marionette.AppRouter.extend({
        appRoutes: {
            //'teams/:id': 'showTeam',
            'projects/:id': 'showProjectStream',
            //'projects/:id/about': 'showProjectAbout',
            //'projects/:id/members': 'showProjectMembers',
            'users/:id': 'showUser'
        }
    });

    var GroupUserController = {};

    var getModel = function (id) {
        var deferred = new $.Deferred();

        if (app.isPrerendering('projects')) {
            deferred.resolve(app.prerenderedView.data);
        } else {
            var params = {};
//            if (id) {
//                params['id'] = id;
//            }
            $.ajax({
                url: '/projects/' + id,
                data: params
            }).done(function (data) {
                deferred.resolve(data.Model);
            });
        }

        return deferred.promise();
    };

    // GroupUserController Public API
    // ------------------------------

    // Show a team
    GroupUserController.showTeam = function () {
        log('team:show');
    };

    // Show project activity
    GroupUserController.showProjectStream = function (id) {
        log('showing projects home stream', this, this);

        $.when(getModel(id))
            .done(function (model) {
                var project = new Project(model.Project);
                var projectLayoutView = new ProjectLayoutView({ model: project });

                app.content[app.getShowViewMethodName('projects')](projectLayoutView);

                if (app.isPrerendering('projects')) {
                    projectLayoutView.showBootstrappedDetails();
                }

                projectLayoutView.showStream();

                app.setPrerenderComplete();
            });
    };

    // Show project observations
    GroupUserController.showProjectObservations = function (id) {
        log('project:show:observations');
        var projectLayoutView = showProjectLayoutView(id);
        //projectLayout.details.showObservations();
    };

    // Show project about
    GroupUserController.showProjectAbout = function (id) {
        log('project:show:about');
        var projectLayoutView = showProjectLayoutView(id);
        //projectLayout.details.showAbout();
    };

    // Show project members
    GroupUserController.showProjectMembers = function (id) {
        log('project:show:members');
        var projectLayoutView = showProjectLayoutView(id);
        //projectLayout.details.showMembers();
    };

    // Show a user
    GroupUserController.showUser = function () {
        log('user:show');
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

    app.vent.on('home:show', function (id) {
        //GroupUserController.showHome(id);
    });

    app.vent.on('project:show:stream', function (id) {
        //GroupUserController.showProjectStream(id);
    });

    app.vent.on('project:show:observations', function (id) {
        //GroupUserController.showProjectObservations(id);
    });

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

    app.addInitializer(function () {
        this.groupUserRouter = new GroupUserRouter({
            controller: GroupUserController
        });
    });

    return GroupUserController;

});