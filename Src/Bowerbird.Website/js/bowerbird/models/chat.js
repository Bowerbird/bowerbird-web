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
            ChatUsers: null,
            ChatMessages: null,
            Type: "Group" // can be "Group" or "Private"
        },

        urlRoot: '/chats',

        idAttribute: 'Id',

        initialize: function (options) {
            this.chatUsers = new UserCollection();
            this.chatMessages = new ChatMessageCollection();
        },

        toJSON: function () {
            return {
                ChatUsers: this.get('ChatUsers'),
                ChatMessages: this.get('ChatMessages')
            };
        },

        addChatUser: function (user) {
            // add new User to chatUsers
        },

        removeChatUser: function (user) {
            // remove User from chatUsers
        },

        addChatMessage: function (message) {
            // add ChatMessage to chatMessages
        }
    });

    return Chat;

});