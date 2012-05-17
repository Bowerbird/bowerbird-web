/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ReferenceSpecies
// ----------------

define(['jquery', 'underscore', 'backbone', 'app'], function ($, _, Backbone, app) {

    var ReferenceSpecies = Backbone.Model.extend({
        defaults: {
            Species: null,
            SmartTags: []
        },

        idAttribute: 'Id',

        url: '/referencespecies/',

        toJSON: function () {
            return {
                Id: this.id,
                Species: this.get('Species'),
                SmartTags: this.get('SmartTags')
            };
        }
    });

    return ReferenceSpecies;

});