/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// OrganisationItemView
// --------------------

// Shows an individual project item
define(['jquery', 'underscore', 'backbone', 'app'], 
function ($, _, Backbone, app) 
{
    var OrganisationItemView = Backbone.Marionette.ItemView.extend({

        tagName: 'li',

        className: 'explore-organisation-item',

        template: 'OrganisationItem',

        events: {
            'click .view-organisation-button': 'viewOrganisation'
        },

        viewOrganisation: function (e) {
            e.preventDefault();
            app.vent.trigger('viewOrganisation:', this.model);
        }
    });

    return OrganisationItemView;
});