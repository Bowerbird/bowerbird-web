/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// PostController & PostRouter
// ---------------------------
define(['jquery', 'underscore', 'backbone', 'app', 'views/postformlayoutview', 'models/post'],
function ($, _, Backbone, app, PostFormLayoutView, Post) 
{
    var PostRouter = Backbone.Marionette.AppRouter.extend({
        appRoutes: {
            'posts/create': 'showPostForm',
            'posts/:id/update': 'showPostForm'
        }
    });

    var PostController = {};

    var getModel = function (id) {
        var url = '/posts/create';
        if (id) {
            url = id;
        }
        var deferred = new $.Deferred();
        if (app.isPrerendering('posts')) {
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

    // Show a post form
    PostController.showPostForm = function (id) {
        log('postController:showPostForm');
        $.when(getModel(id))
            .done(function (model) {
                var post = new Post(model.Post);
                var postFormLayoutView = new PostFormLayoutView({ model: post });

                app.content[app.getShowViewMethodName()](postFormLayoutView);

                if (app.isPrerendering('posts')) {
                    postFormLayoutView.showBootstrappedDetails();
                }

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