/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// Identification
// --------------

define(['jquery', 'underscore', 'backbone'], function ($, _, Backbone) {

    var Identification = Backbone.Model.extend({
        defaults: {
            Id: null,
            SightingId: '',
            Comments: null,
            RankType: null,
            Name: null,
            AllCommonNames: null,
            IsCustomIdentification: false,
            Taxonomy: null,
            Category: null,
            Kingdom: null,
            Phylum: null,
            Class: null,
            Order: null,
            Family: null,
            Genus: null,
            Species: null,
            Subspecies: null,
            CommonGroupNames: [],
            CommonNames: [],
            Synonyms: []
        },

        url: function () {
            var url = '/' + this.get('SightingId') + '/identifications';
            if (this.id) {
                url += '/' + this.id;
            }
            return url;
        },

        idAttribute: 'Id',

        hasTaxonomy: function () {
            if (this.get('IsCustomIdentification') === true) {
                return this.get('Kingdom') !== '';
            } else {
                return this.get('Taxonomy') !== '';
            }
        },

        clearId: function () {
            this.set('IsCustomIdentification', false);
            this.set('RankType', null);
            this.set('Name', null);
            this.set('AllCommonNames', null);
            this.set('Taxonomy', null);
            this.set('Category', null);
            this.set('Kingdom', null);
            this.set('Phylum', null);
            this.set('Class', null);
            this.set('Order', null);
            this.set('Family', null);
            this.set('Genus', null);
            this.set('Species', null);
            this.set('Subspecies', null);
            this.set('CommonGroupNames', []);
            this.set('CommonNames', []);
            this.set('Synonyms', []);
        },

        isValid: function (fireEvent) {
            var isValid = true;
            var errors = [];

            if (!this.hasTaxonomy()) {
                errors.push({ Field: 'Taxonomy', Message: 'Please select an identification.' });
                isValid = false;
            }

            if (fireEvent === true) {
                this.trigger('validated', this, errors);
            }

            return isValid;
        }

    });

    return Identification;

});


