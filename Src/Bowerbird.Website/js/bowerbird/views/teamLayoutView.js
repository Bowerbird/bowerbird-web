/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ProjectLayoutView
// -----------------

define(['jquery', 'underscore', 'backbone', 'app', 'views/streamview', 'collections/streamitemcollection'], 
function ($, _, Backbone, app, StreamView, StreamItemCollection) {

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
            var streamItemCollection = new StreamItemCollection(null, { groupOrUser: this.model });
            var options = {
                model: this.model,
                collection: streamItemCollection
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
            streamItemCollection.fetchFirstPage();
        }
    });

    return TeamLayoutView;

}); 