/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />
/// <reference path="../../libs/moment/moment.js" />

// Timer
// -----

define(['jquery', 'underscore', 'backbone'],
function ($, _, Backbone) {

    var Timer = Backbone.Model.extend({
        defaults: {
            interval: 1 * 10 * 1000,
            timeout: null
        },

        initialize: function (options) {
            _.bindAll(this, 'start', 'tick', 'stop');
            if (options.interval) {
                this.set('interval', options.interval);
            }
        },

        start: function () {
            var timer = setTimeout(this.tick, this.get('interval'));
            this.set('timeout', timer);
        },

        tick: function () {
            var self = this;
            self.trigger('timerexpired', this);
            self.start();
        },

        stop: function () {
            clearTimeout(this.get('timeout'));
        }
    });

    return Timer;
});