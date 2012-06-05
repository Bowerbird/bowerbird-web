/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// OnlineUsersCompositeView
// ----------------------------

define([
'jquery',
'underscore',
'backbone',
'app',
'views/onlineuseritemview'
],
function ($, _, Backbone, app, OnlineUserItemView) {

    var OnlineUsersCompositeView = Backbone.Marionette.CompositeView.extend({

        tagname: 'section',

        id: 'onlineUsers',

        itemView: OnlineUserItemView,

        template: 'UsersOnlineList',

        classname: 'single-1-window',

        regions: {
            summary: '#online-user-summary',
            users: '#online-users'
        },

        events: {

        },

        onRender: function () {
            $('article').append(this.el);
        },

        serializeData: function () {
            return {
                //OnlineUsers: this.model.onlineUsers,
                Count: this.collection.length
            };
        }
    });

    app.addInitializer(function (options) {

        var passedInUsers = options.OnlineUsers;
        var appUsers = app.onlineUsers;

        //var onlineUsersCompositeView = new OnlineUsersCompositeView({ model: { onlineUsers: options.OnlineUsers} });
        //var onlineUsersCompositeView = new OnlineUsersCompositeView({ model: { onlineUsers: app.onlineUsers} });
        var onlineUsersCompositeView = new OnlineUsersCompositeView({ model: app.onlineUsers, collection: app.onlineUsers });

        onlineUsersCompositeView.on('show', function () {
            app.vent.trigger('onlineUsers:rendered');
        });

        app.usersonline.show(onlineUsersCompositeView);
    });

    return OnlineUsersCompositeView;

});