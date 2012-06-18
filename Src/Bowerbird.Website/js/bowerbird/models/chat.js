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

        idAttribute: 'Id',

        initialize: function (attributes, options) {
            log('Chat.initialize');
            this.chatUsers = options.chatUsers;
            this.chatMessages = options.chatMessages;
        },

        //        toJSON: function () {
        //            return {
        //                ChatUsers: this.get('ChatUsers'),
        //                ChatMessages: this.get('ChatMessages')
        //            };
        //        },

        addChatUser: function (user) {
            // todo: Check if exists already
            this.chatUsers.add(user);
        },

        removeChatUser: function (userId) { 
            // todo: Check if exists already
            this.chatUsers.remove(userId);
        },

        addChatMessage: function (message) {
            log('chat.addChatMessage', message);
            // todo: Check if exists already
            this.chatMessages.add(message);
        }
    });

    return Chat;

});