/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />
/// <reference path="../models/observation.js" />

// ObservationController & ObservationRouter
// -----------------------------------------
define(['jquery', 'underscore', 'backbone', 'app', 'views/observationlayoutview', 'models/observation'], 
function ($, _, Backbone, app, ObservationLayoutView, Observation) 
{
    var ObservationRouter = Backbone.Marionette.AppRouter.extend({
        appRoutes: {
            'observations/create': 'showObservationForm',
            'observations/:id/update': 'showObservationForm'
        }
    });

    var ObservationController = {};

    // Helper method to load project layout, taking into account bootstrapped data and prerendered view
    var showObservationLayoutView = function (observation) {
        var observationLayoutView = new ObservationLayoutView({ model: observation });
        //app.content[app.getShowViewMethodName('observations')](observationLayoutView);
        app.showFormContentView(observationLayoutView, 'observations');

        if (app.isPrerendering('observations')) {
            observationLayoutView.showBootstrappedDetails();
        }

        return observationLayoutView;
    };

    var getModel = function (id) {
        var deferred = new $.Deferred();

        if (app.isPrerendering('observations')) {
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

    // Public API
    // ----------

    ObservationController.showObservationForm = function (id) {
        $.when(getModel(id))
            .done(function (model) {
                var observation = new Observation(model.Observation);
                var observationLayoutView = showObservationLayoutView(observation);
                observationLayoutView.showObservationForm(observation, model.Categories);
                app.setPrerenderComplete();
            });
    };

    // Event Handlers
    // --------------

    //    app.vent.on('observation:show', function (id) {
    //        ContributionController.showObservationForm(id);
    //    });

    app.addInitializer(function () {
        this.observationRouter = new ObservationRouter({
            controller: ObservationController
        });
    });

    return ObservationController;
});