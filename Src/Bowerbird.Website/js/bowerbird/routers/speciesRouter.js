/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// SpeciesRouter
// ------------------

define(['jquery', 'underscore', 'backbone', 'app', 'controllers/speciescontroller'], function ($, _, Backbone, app, SpeciesController) {

    var SpeciesRouter = Backbone.Marionette.AppRouter.extend({
        appRoutes: {
            'species/create': 'showSpeciesForm'
        }
    });

    app.addInitializer(function () {
        this.speciesRouter = new SpeciesRouter({
            controller: SpeciesController
        });
    });

    return SpeciesRouter;
});