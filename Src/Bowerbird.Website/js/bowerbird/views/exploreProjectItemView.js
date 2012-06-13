/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ExploreProjectItemView
// ----------------------

// Shows an individual project item
define(['jquery', 'underscore', 'backbone', 'app', 'models/project'], function ($, _, Backbone, app) {

    var ExploreProjectItemView = Backbone.Marionette.ItemView.extend({
        
        className: 'explore-project-item',

        template: 'ProjectItem',

        serializeData: function () {
            var model = this.model.toJSON();
            return {
                Model: model
            };
        }
    });

    return ExploreProjectItemView;

});