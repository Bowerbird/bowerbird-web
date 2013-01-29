/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// UserController & UserRouter
// ---------------------------

define(['jquery', 'underscore', 'backbone', 'app', 'models/user', 'collections/usercollection', 'collections/activitycollection', 'collections/sightingcollection', 'views/userdetailsview', 'views/userexploreview'],
function ($, _, Backbone, app, User, UserCollection, ActivityCollection, SightingCollection, UserDetailsView, UserExploreView) {
    var UserRouter = Backbone.Marionette.AppRouter.extend({
        appRoutes: {
            'users/:id/sightings*': 'showSightings',
            'users/:id/about': 'showAbout',
            'users/:id': 'showUserDetails',
            'users*': 'showExplore'
        }
    });

    var UserHubRouter = function (options) {
        this.userHub = options.hub;
        this.userHub.setupOnlineUsers = setupOnlineUsers;
        this.userHub.userStatusUpdate = userStatusUpdate;
        this.userHub.joinedGroup = joinedGroup;
        this.userHub.mediaResourceUploadSuccess = mediaResourceUploadSuccess;
        this.userHub.mediaResourceUploadFailure = mediaResourceUploadFailure;
        //this.userHub.updateOnlineUsers = updateOnlineUsers; // not called back from server hub anymore..
        this.updateUserClientStatus = updateUserClientStatus;
    };

    var UserController = {};

    var getModel = function (id) {
        var url = '/users/create';
        if (id) {
            url = id;
        }
        var deferred = new $.Deferred();
        if (app.isPrerenderingView('users')) {
            deferred.resolve(app.prerenderedView.data);
        } else {
            $.ajax({
                url: url
            }).done(function (data) {
                deferred.resolve(data.Model);
            });
        }
        return deferred.promise();
    };

    // Hub Callbacks
    // -------------

    // Receive a list of users that are online
    var setupOnlineUsers = function (onlineUsers) {
        log('userController.setupOnlineUsers', this, onlineUsers);
        app.onlineUsers.add(onlineUsers);
    };

    // ping the server with the user's latest activity
    // in responding, 
    var updateUserClientStatus = function (userId, latestHeartbeat, latestInteractivity) {
        log(userId, latestHeartbeat, latestInteractivity);

        this.userHub.updateUserClientStatus(userId, latestHeartbeat, latestInteractivity)
                .done(function (onlineUsers) {
                    log('online users received from server:', onlineUsers);
                    updateOnlineUsers(onlineUsers);
                })
                .fail(function (error) {
                    // TODO: Error handling
                });
    };

    var updateOnlineUsers = function (onlineUsers) {
        log('userController.updateOnlineUsers');
        // Then set their status
        _.each(onlineUsers, function (user) {
            // loop through each of onlineUsers.[] users.. perhaps in a for loop.
            userStatusUpdate(user);
        });
    };

    // This should never be 'called' from the server.
    // Receive a user status update
    var userStatusUpdate = function (user) {
        log('userController.userStatusUpdate', this, user);

        if (!app.onlineUsers.get(user.Id)) {
            app.onlineUsers.add(user);
        }

        // Then set their status
        app.onlineUsers.get(user.Id).set('LatestActivity', user.LatestActivity);
        app.onlineUsers.get(user.Id).set('LatestHeartbeat', user.LatestHeartbeat);
    };

    // Receive a user joined group update
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

    // Receive a media upload success notification
    var mediaResourceUploadSuccess = function (mediaResource) {
        app.vent.trigger('mediaresourceuploadsuccess', mediaResource);
    };

    // Receive a media upload failure notification
    var mediaResourceUploadFailure = function (key, reason) {
        app.vent.trigger('mediaresourceuploadfailure', key, reason);
    };

    // Public API
    // ----------

    UserController.showUserDetails = function (id) {
        // Beacause IE is using has fragments, we have to fix the id manually for IE
        var url = id;
        if (url.indexOf('users') == -1) {
            url = '/users/' + url;
        }

        $.when(getModel(url))
            .done(function (model) {
                var user = new User(model.User);
                var activityCollection = new ActivityCollection(model.Activities.PagedListItems, { id: user.id });
                activityCollection.setPageInfo(model.Activities);

                if (app.content.currentView instanceof UserDetailsView && app.content.currentView.model.id === user.id) {
                    app.content.currentView.showActivity(activityCollection);
                } else {
                    var options = { model: user };
                    if (app.isPrerenderingView('users')) {
                        options['el'] = '.user';
                    }
                    var userDetailsView = new UserDetailsView(options);

                    app.showContentView(user.get('Name'), userDetailsView, 'users', function () {
                        userDetailsView.showActivity(activityCollection);
                    });
                }
            });
    };

    UserController.showSightings = function (id, params) {
        $.when(getModel('/users/' + id + '/sightings?view=' + (params && params.view ? params.view : 'thumbnails') + '&sort=' + (params && params.sort ? params.sort : 'newest')))
        .done(function (model) {
            var user = new User(model.User);
            var sightingCollection = new SightingCollection(model.Sightings.PagedListItems,
                {
                    userId: user.id,
                    page: model.Query.page,
                    pageSize: model.Query.PageSize,
                    total: model.Sightings.TotalResultCount,
                    viewType: model.Query.View,
                    sortBy: model.Query.Sort,
                    category: model.Query.Category,
                    needsId: model.Query.NeedsId,
                    query: model.Query.Query,
                    field: model.Query.Field,
                    taxonomy: model.Query.Taxonomy
                });

            if (app.content.currentView instanceof UserDetailsView && app.content.currentView.model.id === user.id) {
                app.content.currentView.showSightings(sightingCollection, model.CategorySelectList, model.FieldSelectList);
            } else {
                var options = { model: user };
                if (app.isPrerenderingView('users')) {
                    options['el'] = '.user';
                }
                var userDetailsView = new UserDetailsView(options);

                app.showContentView(user.get('Name'), userDetailsView, 'users', function () {
                    userDetailsView.showSightings(sightingCollection, model.CategorySelectList, model.FieldSelectList);
                });
            }
        });
    };

    UserController.showAbout = function (id) {
        $.when(getModel('/users/' + id + '/about'))
        .done(function (model) {
            var user = new User(model.User);

            if (app.content.currentView instanceof UserDetailsView && app.content.currentView.model.id === user.id) {
                app.content.currentView.showAbout(model.ActivityTimeseries);
            } else {
                var options = { model: user };
                if (app.isPrerenderingView('users')) {
                    options['el'] = '.user';
                }
                var userDetailsView = new UserDetailsView(options);

                app.showContentView(user.get('Name'), userDetailsView, 'users', function () {
                    userDetailsView.showAbout(model.ActivityTimeseries);
                });
            }
        });
    };

    // Show user explore
    UserController.showExplore = function (params) {
        $.when(getModel('/users?sort=' + (params && params.sort ? params.sort : 'a-z')))
        .done(function (model) {
            var userCollection = new UserCollection(model.Users.PagedListItems,
                {
                    page: model.Query.page,
                    pageSize: model.Query.PageSize,
                    total: model.Users.TotalResultCount,
                    viewType: model.Query.View,
                    sortBy: model.Query.Sort,
                    query: model.Query.Query,
                    field: model.Query.Field
                });

            var options = {
                userCollection: userCollection,
                fieldSelectList: model.FieldSelectList
            };

            if (app.authenticatedUser) {
                options.model = app.authenticatedUser.user;
            }

            if (app.isPrerenderingView('users')) {
                options['el'] = '.users';
            }
            var userExploreView = new UserExploreView(options);

            app.showContentView('People', userExploreView, 'users', function () {
            });
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
    });

    return UserController;
});