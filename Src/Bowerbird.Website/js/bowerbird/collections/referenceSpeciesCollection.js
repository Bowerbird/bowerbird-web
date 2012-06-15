/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ReferenceSpeciesCollection
// -----------------

define(['jquery', 'underscore', 'backbone', 'models/referencespecies'], function ($, _, Backbone, ReferenceSpecies) {

    var ReferenceSpecies = Backbone.Collection.extend({
        model: ReferenceSpecies,

        url: '/referencespecies',

        initialize: function () {
            _.extend(this, Backbone.Events);
        },

        toJSONViewModel: function () {
            var viewModels = [];
            _.each(this.models, function (referencespecies) {
                viewModels.push(referencespecies.toJSONViewModel());
            });
            return viewModels;
        }
    });

    return ReferenceSpeciesCollection;

});