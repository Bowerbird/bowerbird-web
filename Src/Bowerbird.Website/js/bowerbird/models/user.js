/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />
/// <reference path="../../libs/moment/moment.js" />

// User
// ----

define(['jquery', 'underscore', 'backbone', 'moment', 'models/timer', 'models/tracker'],
function ($, _, Backbone, moment, Timer, Tracker) {

    var User = Backbone.Model.extend({
        defaults: {
            Name: '',
            Email: '',
            AvatarId: null,
            LatestActivity: null,
            LatestHeartbeat: null,
            Timer: null,
            Tracker: null
        },

        idAttribute: 'Id',

        urlRoot: '/users',

        initialize: function () {
            _.bindAll(this, 'startTimer', 'stopTimer', 'timerExpired', 'startTracker', 'stopTracker', 'onLatestActivityChanged', 'onLatestHeartbeatChange', 'getCurrentStatus');
            this.on('change:LatestHeartbeat', this.onLatestHeartbeatChange, this);
        },

        setAvatar: function (mediaResource) {
            this.set('AvatarId', mediaResource.id);
        },

        startTimer: function () {
            log('timer started');
            this.set('Timer', new Timer({ interval: 15000 }));
            this.get('Timer').on('timerexpired', this.timerExpired, this);
            this.get('Timer').start();
        },

        stopTimer: function () {
            if (this.get('Timer')) {
                this.get('Timer').stop();
            }
        },

        timerExpired: function () {
            var self = this;
            log('user.timerExpired');
            self.set('LatestHeartbeat', new Date().toJSON());
        },

        startTracker: function () {
            var self = this;
            self.set('Tracker', new Tracker());
            self.get('Tracker').on('interactivityregisterd', this.onLatestActivityChanged);
            self.get('Tracker').start();
        },

        stopTracker: function () {
            var self = this;
            if (self.get('Tracker')) {
                self.get('Tracker').stop();
            }
        },

        onLatestActivityChanged: function () {
            var self = this;
            var logTime = new Date().toJSON();
            var status = self.getCurrentStatus();
            
            self.set('LatestActivity', logTime);
            if (status === 'away' || status === 'offline') {
                self.get('Timer').tickNow();
            }
        },

        onLatestHeartbeatChange: function () {
            var self = this;
            self.trigger('statuschange', { user: self, status: self.getCurrentStatus() });
            
            if (self.get('LatestActivity')) {
                self.trigger('pollserver', { user: self });
            }
        },
        
        getCurrentStatus: function () {
            var self = this;
            var latestActivity = moment(self.get('LatestActivity'));
            var latestHeartbeat = moment(self.get('LatestHeartbeat'));

            if (latestHeartbeat - latestActivity > 600000) { // ten mins
                return 'offline';
            } else if (latestHeartbeat - latestActivity > 90000) { // one & a half mins
                return 'away';
            } else {
                return 'online';
            }
        }
    });

    return User;

});