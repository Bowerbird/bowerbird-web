/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// UserController & UserRouter
// ---------------------------

define(['jquery', 'underscore', 'backbone', 'app', 'views/userformlayoutview', 'models/user'],
function ($, _, Backbone, app, UserFormLayoutView, User) 
{
    var UserRouter = Backbone.Marionette.AppRouter.extend({
        appRoutes:{
            'users/:id/update': 'showUserForm'
        }
    });

    var UserHubRouter = function(options) {
        this.userHub = options.hub;

        this.userHub.setupOnlineUsers = setupOnlineUsers;
        this.userHub.userStatusUpdate = userStatusUpdate;
        this.userHub.joinedGroup = joinedGroup;
    };

    var UserController = {};

    //var userHub = $.connection.chatHub;

    var getModel = function (id) {
        var deferred = new $.Deferred();
        if (app.isPrerendering('users')) {
            deferred.resolve(app.prerenderedView.data);
        } else {
            var params = {};
            if (id) {
                params['id'] = id;
                $.ajax({
                    url: '/users/' + id,
                    data: params
                }).done(function (data) {
                    deferred.resolve(data.Model);
                });
            } else {
                $.ajax({
                    url: '/users/create'
                }).done(function (data) {
                    deferred.resolve(data.Model);
                });
            }
        }
        return deferred.promise();
    };

    // Hub Callbacks
    // -------------

    // Receive a usee status update
    var userStatusUpdate = function (userStatus) {
        log('activityController.userStatusUpdate', this, userStatus);

        app.onlineUsers.add(userStatus.User);

        // Then set their status
        app.onlineUsers.get(userStatus.User.Id).set('Status', 'Online');
    };

    // Receive a list of users that are online
    var setupOnlineUsers = function (onlineUsers) {
        log('activityController.setupOnlineUsers', this, onlineUsers);
        app.onlineUsers.add(onlineUsers);
    };

    var joinedGroup = function (group) {
        if (group.GroupType === 'project') {
            app.authenticatedUser.projects.add(group);
        }
        if (group.GroupType === 'team') {
            app.authenticatedUser.teams.add(group);
        }
        if (group.GroupType === 'organisation') {
            app.authenticatedUser.organisations.add(group);
        }
    };

    // Show an project form
    UserController.showUserForm = function (id) {
        log('userController:showUserForm');
        $.when(getModel(id))
            .done(function (model) {
                var user = new User(model.User);
                var userFormLayoutView = new UserFormLayoutView({ model: user });
                app.showFormContentView(userFormLayoutView, 'users');
                if (app.isPrerendering('users')) {
                    userFormLayoutView.showBootstrappedDetails();
                }
                app.setPrerenderComplete();
            });
    };

    app.addInitializer(function () {
        log('userController.initialize');
        
        this.userRouter = new UserRouter({
            controller: UserController
        });
        this.userHubRouter = new UserHubRouter({
            hub: $.connection.userHub
        });
        //log(this.userRouter);
    });

    return UserController;

});
