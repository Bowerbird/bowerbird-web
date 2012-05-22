/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ObservationLayoutView
// ---------------------

// Layout of an observation in both edit and view mode
define(['jquery', 'underscore', 'backbone', 'app', 'views/observationformlayoutview'], function ($, _, Backbone, app, ObservationFormLayoutView) {

    var ObservationLayoutView = Backbone.Marionette.Layout.extend({
        className: 'form single-medium observation',

        template: 'Observation',

        regions: {
            main: '.main',
            notes: '.notes',
            comments: '.comments'
        },

        showBootstrappedDetails: function () {
            this.initializeRegions();
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