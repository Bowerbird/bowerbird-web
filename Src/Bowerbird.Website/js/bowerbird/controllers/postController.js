/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// PostController
// --------------

// This is the controller contributions (observations & posts). It contains all of the 
// high level knowledge of how to run the app when it's in contribution mode.
define(['jquery', 'underscore', 'backbone', 'app', 'views/postformlayoutview', 'models/post'], function ($, _, Backbone, app, PostFormLayoutView, Post) {

    var PostController = {};

    var getModel = function (id) {
        var deferred = new $.Deferred();

        if (app.isPrerendering('posts')) {
            deferred.resolve(app.prerenderedView.data);
        } else {
            var params = {};
            if (id) {
                params['id'] = id;
            }
            $.ajax({
                url: '/posts/create',
                data: params
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

    return PostController;

});

//// ProjectController
//// ----------------------

//// This is the controller contributions (observations & posts). It contains all of the 
//// high level knowledge of how to run the app when it's in contribution mode.
//define(['jquery', 'underscore', 'backbone', 'app', 'models/project', 'views/projectformlayoutview'], function ($, _, Backbone, app, Project, ProjectFormLayoutView) {

//    var ProjectController = {};

//    // ProjectController Public API
//    // ---------------------------------

//    // Show a project
//    ProjectController.showProjectForm = function () {

//        var projectFormLayoutView = new ProjectFormLayoutView({ el: $('.project-create-form'), model: new Project(app.prerenderedView.Project) });
//        var editAvatarView = new EditAvatarView({ el: '#avatar-add-pane'});

//        projectFormLayoutView.render();
//        projectFormLayoutView.avatar.show(editAvatarView);

//        app.prerenderedView.isBound = true;
//    };

//    // ProjectController Event Handlers
//    // -------------------------------------

//    app.vent.on('project:show', function () {
//        ProjectController.showProjectForm();
//    });

//    return ProjectController;

//});