/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ContributionController
// ----------------------

// This is the controller contributions (observations & posts). It contains all of the 
// high level knowledge of how to run the app when it's in contribution mode.
define(['jquery', 'underscore', 'backbone', 'app', 'views/observationlayoutview', 'models/observation', 'views/observationformlayoutview'], function ($, _, Backbone, app, ObservationLayoutView, Observation, ObservationFormLayoutView) {

    var ContributionController = {};

    // Helper method to load project layout, taking into account bootstrapped data and prerendered view
    var showObservationLayoutView = function (id) {
        //app.vent.trigger('observation:show');
        var observationLayoutView = null;
        if (app.prerenderedView.name === 'observations' && !app.prerenderedView.isBound) {
            observationLayoutView = new ObservationLayoutView({ model: new Observation(app.prerenderedView.Observation) });
            app.content.attachView(observationLayoutView);
            //app.prerenderedView.isBound = true;
        } else {
            if (id) {
                observationLayoutView = new ObservationLayoutView(); // TODO: Get observation
            } else {
                observationLayoutView = new ObservationLayoutView({ model: new Observation() });
            }
            app.content.show(observationLayoutView);
        }
        observationLayoutView.render();
        return observationLayoutView;
    };

    // ContributionController Public API
    // ---------------------------------

    // Show an observation
    ContributionController.showObservationForm = function (id) {
        var observationLayoutView = showObservationLayoutView(id);

        var observationFormLayoutView = new ObservationFormLayoutView({ el: $('.observation-create-form'), model: observationLayoutView.model });

        if (app.prerenderedView.name === 'observations' && !app.prerenderedView.isBound) {
            observationLayoutView.main.attachView(observationFormLayoutView);
        } else {
            observationLayoutView.main.show(observationFormLayoutView);
        }
        observationFormLayoutView.render();

        app.prerenderedView.isBound = true;
    };

    // ContributionController Event Handlers
    // -------------------------------------

    app.vent.on('observation:show', function (id) {
        ContributionController.showObservationForm(id);
    });

    return ContributionController;

});
