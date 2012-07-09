/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ObservationLayoutView
// ---------------------

// Layout of an observation in both edit and view mode
define(['jquery', 'underscore', 'backbone', 'app', 'views/observationdetailsview', 'views/observationformlayoutview'], function ($, _, Backbone, app, ObservationDetailsView, ObservationFormLayoutView) {

    var ObservationLayoutView = Backbone.Marionette.Layout.extend({
        className: 'observation',

        template: 'Observation',

        regions: {
            main: '.main',
            notes: '.notes',
            comments: '.comments'
        },

        showBootstrappedDetails: function () {
            this.initializeRegions();
            this.$el = $('#content .observation');
        },

        showObservationDetails: function (observation) {
            var options = { model: observation };

            if (app.isPrerendering('observations')) {
                options['el'] = '.observation-details';
            }

            var observationDetailsView = new ObservationDetailsView(options);
            this.main[app.getShowViewMethodName('observations')](observationDetailsView);

            if (app.isPrerendering('observations')) {
                observationDetailsView.showBootstrappedDetails();
            }
        },

        showObservationForm: function (observation, categories) {
            var options = { model: observation, categories: categories };

            if (app.isPrerendering('observations')) {
                options['el'] = '.observation-form';
            }

            var observationFormLayoutView = new ObservationFormLayoutView(options);
            this.main[app.getShowViewMethodName('observations')](observationFormLayoutView);

            if (app.isPrerendering('observations')) {
                observationFormLayoutView.showBootstrappedDetails();
            }
        }
    });

    return ObservationLayoutView;

});