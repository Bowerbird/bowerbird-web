/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// PostController & PostRouter
// ---------------------------

define(['jquery', 'underscore', 'backbone', 'app', 'models/post', 'views/postdetailsview', 'views/postformview'],
function ($, _, Backbone, app, Post, PostDetailsView, PostFormView) {
    var PostRouter = Backbone.Marionette.AppRouter.extend({
        appRoutes: {
            'projects/:projectId/posts/create': 'showProjectPostCreateForm',
            'organisations/:organisationId/posts/create': 'showOrganisationPostCreateForm',
            'projects/:projectId/posts/:postId/update': 'showProjectPostUpdateForm',
            'organisations/:organisationId/posts/:postId/update': 'showOrganisationPostUpdateForm',
            'projects/:projectId/posts/:postId': 'showProjectPostDetails',
            'organisations/:organisationId/posts/:postId': 'showOrganisationPostDetails'
        }
    });

    var PostController = {};

    var showPostForm = function (uri) {
        $.when(getModel(uri, 'posts'))
            .done(function (model) {
                var post = new Post(model.Post);

                var options = { model: post, categorySelectList: model.CategorySelectList, categories: model.Categories, projectsSelectList: model.ProjectsSelectList };

                if (app.isPrerenderingView('posts')) {
                    options['el'] = '.post-form';
                }

                var postFormView = new PostFormView(options);
                app.showContentView('Edit Post', postFormView, 'posts');
            });
    };

    var showPostDetails = function (url) {
        // Beacause IE is using hash fragments, we have to fix the id manually for IE
//        var url = id;
//        if (url.indexOf('posts') == -1) {
//            url = '/posts/' + url;
//        }

        $.when(getModel(url, 'posts'))
            .done(function (model) {
                var post = new Post(model.Post);

                var options = {
                    model: post
                };

                if (app.isPrerenderingView('posts')) {
                    options['el'] = '.post';
                }

                var postDetailsView = new PostDetailsView(options);
                app.showContentView(post.get('Subject'), postDetailsView, 'posts');
            });
    };

    var getModel = function (uri, viewName) {
        var deferred = new $.Deferred();
        if (app.isPrerenderingView(viewName)) {
            deferred.resolve(app.prerenderedView.data);
        } else {
            $.ajax({
                url: uri
            }).done(function (data) {
                deferred.resolve(data.Model);
            });
        }
        return deferred.promise();
    };

    // Public API
    // ----------

    PostController.showProjectPostDetails = function (projectId, postId) {
        showPostDetails('/projects/' + projectId + '/posts/' + postId);
    };

    PostController.showOrganisationPostDetails = function (organisationId, postId) {
        showPostDetails('/organisations/' + organisationId + '/posts/' + postId);
    };

    PostController.showProjectPostCreateForm = function (projectId) {
        showPostForm('/projects/' + projectId + '/posts/create');
    };

    PostController.showOrganisationPostCreateForm = function (organisationId) {
        showPostForm('/organisations/' + organisationId + '/posts/create');
    };

    PostController.showProjectPostUpdateForm = function (projectId, postId) {
        showPostForm('/projects/' + projectId + '/posts/' + postId + '/update');
    };

    PostController.showOrganisationPostUpdateForm = function (organisationId, postId) {
        showPostForm('/organisations/' + organisationId + '/posts/' + postId + '/update');
    };

//    PostController.mediaResourceUploaded = function (e, mediaResource) {
//        app.vent.trigger('mediaResourceUploaded:', mediaResource);
//    };

    // Event Handlers
    // --------------

    app.addInitializer(function () {
        this.postRouter = new PostRouter({
            controller: PostController
        });
    });

    return PostController;
});