/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// Species
// -------

define(['jquery', 'underscore', 'backbone', 'app'], function ($, _, Backbone, app) {

    var Species = Backbone.Model.extend({
        defaults: {
            Kingdom: '',
            Group: '',
            CommonNames: [],
            Taxonomy: '',
            Order: '',
            Family: '',
            GenusName: '',
            SpeciesName: '',
            ProposedAsNewSpecies: false
        },

        idAttribute: 'Id',

        url: '/species/',

        toJSON: function () {
            return {
                Id: this.id,
                Kingdom: this.get('Kingdom'),
                Group: this.get('Group'),
                CommonNames: this.get('CommonNames'),
                Taxonomy: this.get('Taxonomy'), // TODO: Fix this to return id?
                Order: this.get('Order'),
                Family: this.get('Family'),
                GenusName: this.get('GenusName'),
                SpeciesName: this.get('SpeciesName'),
                ProposedAsNewSpecies: this.get('ProposedAsNewSpecies')
            };
        }
    });

    return Species;

});