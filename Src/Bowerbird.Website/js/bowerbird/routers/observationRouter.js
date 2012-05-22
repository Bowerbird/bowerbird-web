/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ObservationRouter
// -----------------

define(['jquery', 'underscore', 'backbone', 'app', 'controllers/observationcontroller'], function ($, _, Backbone, app, ObservationController) {

    var ObservationRouter = Backbone.Marionette.AppRouter.extend({
        appRoutes: {
            'observations/create': 'showObservationForm',
            'observations/:id/update': 'showObservationForm'
        }
    });

    app.addInitializer(function () {
        this.observationRouter = new ObservationRouter({
            controller: ObservationController
        });
    });

    return ObservationRouter;

});
