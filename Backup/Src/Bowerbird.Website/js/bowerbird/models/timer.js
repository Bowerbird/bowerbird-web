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
            interval: 15 * 60 * 1000, // mins * sec * millisec (15 mins polling turned on for debugging purposes)
            timeout: null
        },

        initialize: function (options) {
            _.bindAll(this, 'start', 'tick', 'stop', 'tickNow');
            if (options.interval) {
                this.set('interval', options.interval);
            }
        },

        /*
        start: function () {
            var self = this;
            var timer = setTimeout(self.tick, self.get('interval'));
            this.set('timeout', timer);
        },
        */

        // http://stackoverflow.com/questions/13876457/signalr-javascript-client-side-timer-hammering-the-server-for-connections
        start: function () {
            var self = this;
            if (this.get('timeout') !== null) {
                //throw "start must not be called when the Timer is already running";
                log('Timout already exists. Canceling and starting again.');
                self.stop();
                self.start();
            }
            else {
                // creating a closure ensures this is bound correctly in tick()
                var timer = setTimeout(function () {
                    self.tick();
                }, this.get('interval'));
                this.set('timeout', timer);
                log('Timer started and counting down....');
            }
        },

        tick: function () {
            var self = this;
            self.stop();
            self.trigger('timerexpired', this);
            self.start();
        },

        tickNow: function () {
            var self = this;
            self.stop();
            self.trigger('timerexpired', this);
            self.start();
        },

        stop: function () {
            var self = this;
            clearTimeout(self.get('timeout'));
            this.set('timeout', null);
        }
    });

    return Timer;
});