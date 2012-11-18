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
            HasIdentification: false,
            Taxonomy: ''
        },

        initialize: function (attributes) {
            if (attributes && attributes.Name) {
                var allCommonNames = _.union(attributes.CommonGroupNames, attributes.CommonNames);

                this.set('HasIdentification', attributes.Category != null);
                this.set('Category', attributes.Category != null ? attributes.Category : '');
                this.set('Name', attributes.Name);
                this.set('RankType', attributes.RankType);
                this.set('Taxonomy', attributes.Taxonomy);
                this.set('HasCommonNames', allCommonNames.length > 0);
                this.set('CommonNames', allCommonNames.join(', '));
            }
        }
    });

    return Identification;

});


