/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// PostController & PostRouter
// ---------------------------
define(['jquery', 'underscore', 'backbone', 'app', 'views/postlayoutview', 'models/post', 'queryparams'],
function ($, _, Backbone, app, PostLayoutView, Post) {
    var PostRouter = Backbone.Marionette.AppRouter.extend({
        appRoutes: {
            'posts/create': 'showCreatePostForm',
            'posts/:id/update': 'showUpdatePostForm',
            'posts/:id': 'showPostDetails'
        }
    });

    var PostController = {};

    var showPostLayoutView = function (post) {
        var postLayoutView = new PostLayoutView({ model: post });
        app.showFormContentView(postLayoutView, 'posts');
        if (app.isPrerenderingView('posts')) {
            postLayoutView.showBootstrappedDetails();
        }
        return postLayoutView;
    };

    var getModel = function (id) {
        var url = '/posts/create';
        if (id) {
            url = id;
        }
        var deferred = new $.Deferred();
        if (app.isPrerenderingView('posts')) {
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

    var createModel = function (groupId) {
        var url = '/posts/create?id=' + groupId;
        var deferred = new $.Deferred();
        if (app.isPrerenderingView('posts')) {
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

    // PostController Public API
    // ----------------------------

    PostController.showPostDetails = function (id) {
        $.when(getModel(id))
            .done(function (model) {
                var post = new Post(model.Post);
                app.updateTitle(post.get('Subject'));
                var postLayoutView = showPostLayoutView(post);
                postLayoutView.showPostDetails(post);
                app.setPrerenderComplete();
            });
    };

    // Show a post form
    PostController.showUpdatePostForm = function (id) {
        log('postController:showPostForm');
        $.when(getModel(id))
            .done(function (model) {
                var post = new Post(model.Post);
                app.updateTitle('Edit Post');
                var postLayoutView = showPostLayoutView(post);
                postLayoutView.showPostForm(post);
                app.setPrerenderComplete();
            });
    };

    // Show a post form
    PostController.showCreatePostForm = function (params) {
        log('postController:showPostForm');
        $.when(createModel(params.id))
            .done(function (model) {
                var post = new Post(model.Post);
                app.updateTitle('New Post');
                var postLayoutView = showPostLayoutView(post);
                postLayoutView.showPostForm(post);
                app.setPrerenderComplete();
            });
    };

    app.addInitializer(function () {
        this.postRouter = new PostRouter({
            controller: PostController
        });
    });

    return PostController;
});