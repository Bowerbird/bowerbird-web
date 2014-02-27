/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// IdentificationCollection
// ------------------------

define(['jquery', 'underscore', 'backbone', 'models/identification'], function ($, _, Backbone, Identification) {

    var IdentificationCollection = Backbone.Collection.extend({
        model: Identification,

        url: '/identifications',

        initialize: function () {
            _.extend(this, Backbone.Events);
        }
    });

    return IdentificationCollection;

});