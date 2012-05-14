/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ProjectCollection
// -----------------

define(['jquery', 'underscore', 'backbone', 'app', 'models/project'], function ($, _, Backbone, app, Project) {

    var ProjectCollection = Backbone.Collection.extend({
        model: Project,

        url: '/projects',

        initialize: function () {
            _.extend(this, Backbone.Events);
        },

        toJSONViewModel: function () {
            var viewModels = [];
            _.each(this.models, function (project) {
                viewModels.push(project.toJSONViewModel());
            });
            return viewModels;
        }
    });

    return ProjectCollection;

});