/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />
/// <reference path="../../libs/moment/moment.js" />

// Tracker
// -------

define(['jquery', 'underscore', 'backbone'],
function ($, _, Backbone) {

    var Tracker = Backbone.Model.extend({
        defaults: {
        },

        initialize: function (options) {
            _.bindAll(this, 'start', 'track', 'stop');
        },

        start: function () {
            $(document).bind('mousemove', this.track);
            $(document).bind('keypress', this.track);
        },

        track: function () {
            var self = this;
            self.trigger('interactivityregisterd');
        },

        stop: function () {
            $(document).unbind('mousemove', false);
            $(document).unbind('keypress', false);
        }
    });

    return Tracker;
});