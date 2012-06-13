/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ReferenceSpeciesController
// ----------------------

define(['jquery','underscore','backbone','app','models/referencespecies','views/referencespeciesformlayoutview'],
function ($,_,Backbone,app,ReferenceSpecies,ReferenceSpeciesFormLayoutView) 
{
    var ReferenceSpeciesController = {};

    // ReferenceSpeciesController Public API
    // ---------------------------------

    // Show a ReferenceSpecies
    ReferenceSpeciesController.showReferenceSpeciesForm = function () {

        var referenceSpeciesFormLayoutView = new ReferenceSpeciesFormLayoutView({ el: $('.reference-species-create-form'), model: new ReferenceSpecies(app.prerenderedView.ReferenceSpecies) });

        referenceSpeciesFormLayoutView.render();

        app.prerenderedView.isBound = true;
    };

    // ReferenceSpecies Event Handlers
    // -------------------------------------

    app.vent.on('referencespecies:show', function () {
        ReferenceSpeciesController.showReferenceSpeciesForm();
    });

    return ReferenceSpeciesController;

});

// ReferenceSpeciesRouter
// ------------------
define(['jquery','underscore','backbone','app','controllers/referencespeciescontroller'],
function ($,_,Backbone,app,ReferenceSpeciesController)
{
    var ReferenceSpeciesRouter = Backbone.Marionette.AppRouter.extend({
        appRoutes: {
            'referencespecies/create': 'showReferenceSpeciesForm'
        }
    });

    app.addInitializer(function () {
        this.referenceSpeciesRouter = new ReferenceSpeciesRouter({
            controller: ReferenceSpeciesController
        });
    });
});