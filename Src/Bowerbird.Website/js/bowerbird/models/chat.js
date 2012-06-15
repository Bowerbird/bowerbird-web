/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// Chat
// ----

define(['jquery', 'underscore', 'backbone', 'app', 'collections/usercollection', 'collections/chatmessagecollection', 'models/user', 'models/chatmessage'],
function ($, _, Backbone, app, UserCollection, ChatMessageCollection, User, ChatMessage) {

    var Chat = Backbone.Model.extend({
        defaults: {
        },

        //urlRoot: '/chats',

        idAttribute: 'id',

        initialize: function (options) {
            log('Chat.initialize');
            this.Title = options.Title;
            this.ChatUsers = options.ChatUsers;
            this.Messages = options.ChatMessages;
            this.chatId = options.ChatId;
        },

        //        toJSON: function () {
        //            return {
        //                ChatUsers: this.get('ChatUsers'),
        //                ChatMessages: this.get('ChatMessages')
        //            };
        //        },

        addChatUser: function (user) {
            // todo: Check if exists already
            this.ChatUsers.add(user);
        },

        removeChatUser: function (user) {
            // todo: Check if exists already
            this.ChatUsers.remove(user);
        },

        addChatMessage: function (message) {
            log('chat.addChatMessage', message);
            // todo: Check if exists already
            var chatMessage = new ChatMessage(message);
            this.Messages.add(chatMessage);
        }
    });

    return Chat;

});