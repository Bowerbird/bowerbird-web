/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ProjectItemView
// ---------------

// Shows an individual project item
define(['jquery', 'underscore', 'backbone', 'app'],
function ($, _, Backbone, app) 
{
    var ProjectItemView = Backbone.Marionette.ItemView.extend({

        tagName: 'li',

        className: 'explore-project-item',

        template: 'ProjectItem',

        events: {
            'click .join-project-button': 'joinProject',
            'click .view-project-button': 'viewProject'
        },

        joinProject: function (e) {
            e.preventDefault();
            app.vent.trigger('joinProject:', this.model);
        },

        viewProject: function (e) {
            e.preventDefault();
            app.vent.trigger('viewProject:', this.model);
        }
    });

    return ProjectItemView;
});