/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// OrganisationLayoutView
// ----------------------

define(['jquery', 'underscore', 'backbone', 'app', 'views/streamview', 'collections/activitycollection'], 
function ($, _, Backbone, app, StreamView, ActivityCollection) {

    var OrganisationLayoutView = Backbone.Marionette.Layout.extend({
        
        className: 'organisation',

        template: 'Organisation',

        regions: {
            summary: '.summary',
            details: '.details'
        },

        showBootstrappedDetails: function () {
            this.initializeRegions();
            this.$el = $('#content .organisation');
        },

        showStream: function () {
            var activityCollection = new ActivityCollection(null, { groupOrUser: this.model });
            var options = {
                model: this.model,
                collection: activityCollection
            };
            if (app.isPrerendering('organisations')) {
                options['el'] = '.stream';
            }
            var streamView = new StreamView(options);
            if (app.isPrerendering('organisations')) {
                this.details.attachView(streamView);
                streamView.showBootstrappedDetails();
            } else {
                this.details.show(streamView);
            }
            activityCollection.fetchFirstPage();
        }
    });

    return OrganisationLayoutView;
}); 