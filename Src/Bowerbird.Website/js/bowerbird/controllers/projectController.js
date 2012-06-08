/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ProjectController
// -----------------

// This is the controller contributions (observations & posts). It contains all of the 
// high level knowledge of how to run the app when it's in contribution mode.
define(['jquery', 'underscore', 'backbone', 'app', 'views/projectformlayoutview', 'models/project'], function ($, _, Backbone, app, ProjectFormLayoutView, Project) {

    var ProjectController = {};

    var getModel = function (id) {
        var deferred = new $.Deferred();

        if (app.isPrerendering('projects')) {
            deferred.resolve(app.prerenderedView.data);
        } else {
            var params = {};
            if (id) {
                params['id'] = id;
            }
            $.ajax({
                url: '/projects/create',
                data: params
            }).done(function (data) {
                deferred.resolve(data.Model);
            });
        }

        return deferred.promise();
    };

    // ProjectController Public API
    // ----------------------------

    // Show an project form
    ProjectController.showProjectForm = function (id) {
        log('projectController:showProjectForm');
        $.when(getModel(id))
            .done(function (model) {
                var project = new Project(model.Project);
                var projectFormLayoutView = new ProjectFormLayoutView({ model: project, teams: model.Teams });

                app.content[app.getShowViewMethodName('projects')](projectFormLayoutView);

                if (app.isPrerendering('projects')) {
                    projectFormLayoutView.showBootstrappedDetails();
                }

                app.setPrerenderComplete();
            });
        };

    return ProjectController;

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