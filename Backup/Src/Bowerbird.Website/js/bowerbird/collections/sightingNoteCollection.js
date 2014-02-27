/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// SightingNoteCollection
// ----------------------

define(['jquery', 'underscore', 'backbone', 'models/sightingnote'], function ($, _, Backbone, SightingNote) {

    var SightingNoteCollection = Backbone.Collection.extend({
        model: SightingNote,

        url: '/sightingnotes',

        initialize: function () {
            _.extend(this, Backbone.Events);
        }
    });

    return SightingNoteCollection;

});