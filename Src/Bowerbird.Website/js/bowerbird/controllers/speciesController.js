/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// SpeciesController & SpeciesRouter
// ---------------------------------
define(['jquery', 'underscore', 'backbone', 'app', 'models/species', 'views/speciesformitemview'],
function ($, _, Backbone, app, Species, SpeciesFormItemView) 
{
    var SpeciesRouter = Backbone.Marionette.AppRouter.extend({
        appRoutes: {
            'species/create': 'showSpeciesForm'
        }
    });

    var SpeciesController = {};

    var getModel = function (id) {
        var url = '';
        if (!id) {
            url = '/species';
        } else {
            url = '/' + id;
        }
        var deferred = new $.Deferred();
        if (app.isPrerenderingView('species')) {
            deferred.resolve(app.prerenderedView.data);
        } else {
            $.ajax({
                url: url
            }).done(function (data) {
                deferred.resolve(data.Model);
            });
        }
        return deferred.promise();
    };

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

    app.addInitializer(function () {
        this.speciesRouter = new SpeciesRouter({
            controller: SpeciesController
        });
    });

    return SpeciesController;

});