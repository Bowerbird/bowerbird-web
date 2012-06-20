/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// TeamItemView
// ------------

// Shows an individual project item
define(['jquery', 'underscore', 'backbone', 'app'], 
function ($, _, Backbone, app) 
{
    var TeamItemView = Backbone.Marionette.ItemView.extend({

        tagName: 'li',

        className: 'explore-team-item',

        template: 'TeamItem',

        events: {
            'click .join-project-button': 'joinProject'
        },

        joinProject: function (e) {
            e.preventDefault();
            app.vent.trigger('joinProject:', this.model);
        }
    });

    return TeamItemView;
});