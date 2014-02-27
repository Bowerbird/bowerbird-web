/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ObservationMediaCollection
// --------------------------

define(['jquery', 'underscore', 'backbone', 'models/observationmedia'], function ($, _, Backbone, ObservationMedia) {

    var ObservationMediaCollection = Backbone.Collection.extend({
        model: ObservationMedia,

        initialize: function () {
            _.extend(this, Backbone.Events);
        }
    });

    return ObservationMediaCollection;

});