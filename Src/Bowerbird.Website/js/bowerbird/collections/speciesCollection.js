/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// SpeciesCollection
// -----------------

define(['jquery', 'underscore', 'backbone', 'app', 'models/species'], function ($, _, Backbone, app, Species) {

    var SpeciesCollection = Backbone.Collection.extend({
        model: Species,

        url: '/species',

        initialize: function () {
            _.extend(this, Backbone.Events);
        },

        toJSONViewModel: function () {
            var viewModels = [];
            _.each(this.models, function (species) {
                viewModels.push(species.toJSONViewModel());
            });
            return viewModels;
        }
    });

    return SpeciesCollection;

});