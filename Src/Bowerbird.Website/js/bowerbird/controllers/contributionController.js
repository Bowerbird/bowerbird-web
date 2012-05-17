/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />
/// <reference path="../models/observation.js" />

// ContributionController
// ----------------------

// This is the controller contributions (observations & posts). It contains all of the 
// high level knowledge of how to run the app when it's in contribution mode.
define(['jquery', 'underscore', 'backbone', 'app', 'views/observationlayoutview', 'models/observation', 'views/observationformlayoutview'], function ($, _, Backbone, app, ObservationLayoutView, Observation, ObservationFormLayoutView) {

    var ContributionController = {};

    // Helper method to load project layout, taking into account bootstrapped data and prerendered view
    var showObservationLayoutView = function (observation) {
        var observationLayoutView = new ObservationLayoutView({ model: observation });
        app.content[getShowViewMethodName()](observationLayoutView);

        if (isPrerender()) {
            observationLayoutView.showBootstrappedDetails();
        }

        return observationLayoutView;
    };

    var isPrerender = function () {
        return app.prerenderedView.name === 'observations' && !app.prerenderedView.isBound;
    };

    var getModel = function (id) {
        var deferred = new $.Deferred();

        if (isPrerender()) {
            deferred.resolve(app.prerenderedView.data);
        } else {
            var params = {};
            if (id) {
                params['id'] = id;
            }
            $.ajax({
                url: '/observations/create',
                data: params,
            }).done(function (data) {
                deferred.resolve(data.Model);
            });
        }

        return deferred.promise();
    };

    var setPrerenderComplete = function () {
        app.prerenderedView.isBound = true;
    };

    var getShowViewMethodName = function () {
        return isPrerender() ? 'attachView' : 'show';
    };

    // ContributionController Public API
    // ---------------------------------

    // Show an observation form
    ContributionController.showObservationForm = function (id) {
        $.when(getModel(id))
            .done(function (model) {
                var observation = new Observation(model.Observation);
                var observationLayoutView = showObservationLayoutView(observation);
                var options = { model: observation, categories: model.Categories };

                if (isPrerender()) {
                    options['el'] = '.observation-form';
                }

                var observationFormLayoutView = new ObservationFormLayoutView(options);
                observationLayoutView.main[getShowViewMethodName()](observationFormLayoutView);

                if (isPrerender()) {
                    observationFormLayoutView.showBootstrappedDetails();
                }

                setPrerenderComplete();
            });
    };

    // ContributionController Event Handlers
    // -------------------------------------

    //    app.vent.on('observation:show', function (id) {
    //        ContributionController.showObservationForm(id);
    //    });

    return ContributionController;

});
