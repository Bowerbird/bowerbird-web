/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />
/// <reference path="../../libs/moment/moment.js" />

// User
// ----

define(['jquery', 'underscore', 'backbone', 'moment'], function ($, _, Backbone, moment) {

    var User = Backbone.Model.extend({
        defaults: {
            FirstName: '',
            LastName: '',
            Email: '',
            AvatarId: null,
            SessionLatestActivity: null
        },

        idAttribute: 'Id',

        urlRoot: '/users',

        initialize: function () {
            _.bindAll(this, 'checkStatus');
            this.on('change:SessionLatestActivity', this.onSessionLatestActivityChange, this);
        },

        setAvatar: function (mediaResource) {
            this.set('AvatarId', mediaResource.id);
        },

        enableCheckStatus: function () {
            this.checkStatus();
        },

        onSessionLatestActivityChange: function (user, sessionLatestActivity) {
            log('user: ' + this.get('Name') + '; latestactivity: ' + sessionLatestActivity, this);
            this.trigger('statuschange', { user: this, status: this.getCurrentStatus() });
        },

        checkStatus: function () {
            log('user: ' + this.get('Name') + '; latestactivity: ' + this.get('SessionLatestActivity'), this);
            this.trigger('statuschange', { user: this, status: this.getCurrentStatus() });
            var that = this;
            this.statusTimerId = setTimeout(that.checkStatus, 20000);
            log('statusTimerId', this.statusTimerId);
        },

        getCurrentStatus: function () {
            var latestActivity = moment(this.get('SessionLatestActivity'));
            var now = moment(new Date);
            if (now - latestActivity > 500000) {
                return 'offline';
            } else if (now - latestActivity > 300000) {
                return 'away';
            } else {
                return 'online';
            }
        }
    });

    return User;

});