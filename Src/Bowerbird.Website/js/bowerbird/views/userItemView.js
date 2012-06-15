/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// OnlineUserItemView
// ------------------

define(['jquery', 'underscore', 'backbone', 'app', 'models/user'], function ($, _, Backbone, app) {

    var UserItemView = Backbone.Marionette.ItemView.extend({

        tagName: 'li',

        className: 'online-user',

        template: 'UserItem',

        events: {
            'click .user-chat-icon': 'startChat'
        },

        onRender: function () {

        },

        serializeData: function () {
            return {
                Id: this.model.id,
                Name: this.model.get('Name'),
                Avatar: this.model.get('Avatar'),
                Type: 'User'
            };
        },

        startChat: function (e) {
            // call can come from a user's chat-icon
            var id = e.target["id"].split('-')[1];
            //var user = app.onlineUsers.get(id);
            //var chatId = this.generateGuid();
            //var chat = new Bowerbird.Models.UserChat({ Id: chatId, User: user });
            //app.chats.add(chat);
            //app.chatRouter.joinChat(chat);
            app.vent.trigger('chats:startPrivateChat', id);
        },

        generateGuid: function () {
            var S4 = function () {
                return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
            };
            return (S4() + S4() + "-" + S4() + "-" + S4() + "-" + S4() + "-" + S4() + S4() + S4());
        }

    });

    return UserItemView;

});
