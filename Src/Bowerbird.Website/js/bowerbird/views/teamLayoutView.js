/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// TeamLayoutView
// -----------------

define(['jquery', 'underscore', 'backbone', 'app', 'views/sightinglistview', 'collections/activitycollection'], 
function ($, _, Backbone, app, StreamView, ActivityCollection) {

    var TeamLayoutView = Backbone.Marionette.Layout.extend({
        className: 'team',

        template: 'Team',

        regions: {
            summary: '.summary',
            details: '.details'
        },

        showBootstrappedDetails: function () {
            this.initializeRegions();
            this.$el = $('#content .team');
        },

        showStream: function () {
            var activityCollection = new ActivityCollection(null, { groupOrUser: this.model });
            var options = {
                model: this.model,
                collection: activityCollection
            };
            if (app.isPrerendering('teams')) {
                options['el'] = '.stream';
            }
            var streamView = new StreamView(options);
            if (app.isPrerendering('teams')) {
                this.details.attachView(streamView);
                streamView.showBootstrappedDetails();
            } else {
                this.details.show(streamView);
            }
            activityCollection.fetchFirstPage();
        }
    });

    return TeamLayoutView;

}); 