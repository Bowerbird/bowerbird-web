/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// SpeciesController
// ----------------------

// This is the controller contributions (observations & posts). It contains all of the 
// high level knowledge of how to run the app when it's in contribution mode.
define(['jquery', 'underscore', 'backbone', 'app', 'models/species', 'views/speciesformitemview'], function ($, _, Backbone, app, Species, SpeciesFormItemView) {

    var SpeciesController = {};

    // SpeciesController Public API
    // ---------------------------------

    // Show a species
    SpeciesController.showSpeciesForm = function () {

        var speciesFormItemView = new SpeciesFormItemView({ el: $('.species-create-form'), model: new Species(app.prerenderedView.Species) });

        speciesFormItemView.render();

        app.prerenderedView.isBound = true;
    };

    // SpeciesController Event Handlers
    // -------------------------------------

    app.vent.on('species:show', function () {
        SpeciesController.showSpeciesForm();
    });

    return SpeciesController;

});