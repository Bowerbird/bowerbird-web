/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// OnlineUsersCompositeView
// ----------------------------

// A collection of links in the sidebar
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
            //'click .online-user-options .start-chat': 'startUserChat',
            //'click .online-user-options .view-profile': 'viewUserProfile'
        },

        onRender: function(){
            $('article').append(this.el);
        },

        serializeData: function () {
            return {
                OnlineUsers: this.model.onlineUsers
            };
        }
    });

    app.addInitializer(function (options) {
        // Only show online users if user is authenticated
        if (this.user) {
            
            // Render the layout and get it on the screen, first
            var onlineUsersCompositeView = new OnlineUsersCompositeView({ model: { onlineUsers: options.OnlineUsers } });

            onlineUsersCompositeView.on('show', function () {
                app.vent.trigger('onlineUsers:rendered');
            });

            app.usersonline.show(onlineUsersCompositeView);
        }
    });


    return OnlineUsersCompositeView;

});