/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ChatView
// --------

// Shows chat window for a chat
define(['jquery', 'underscore', 'backbone', 'app', 'ich', 'views/chatmessageitemview'],
function ($, _, Backbone, app, ich, ChatMessageItemView) 
{
    var ChatCompositeView = Backbone.Marionette.CompositeView.extend({

        template: "Chat",

        itemView: ChatMessageItemView,

        className: 'chat',

        events: {
            "click .chat-send-message-button": "sendMessage",
            "click .window-close": "closeWindow"
        },

        regions: {
            users: '#chat-users'
        },

        onRender: function () {
            


            this.$el.append(ich.ChatWindow({ Title: this.chat.get('Title'), Messages: this.chat.chatMessages.toJSON(), ChatUsers: this.chat.chatUsers.toJSON() }));
            return this;
        },

        sendMessage: function () {
            log('chatView.sendMessage');
            app.chatRouter.sendMessage(this.$el.find('.new-chat-message').val(), this.chat);
            this.$el.find('.new-chat-message').val('');
        },

        closeWindow: function () {
            app.chats.remove(this.chat);
            this.$el.remove();
        },

        addChatMessage: function (chatMessage) {
            log('chatView.addChatMessage');
            this.$el.find('.chat-messages').append(ich.ChatMessage(chatMessage.toJSON()));
        },

        changeUsers: function () {
            var users = this.chat.chatUsers.toJsonViewModel();
            this.$el.find('.chat-current-users').empty();
            this.$el.find('.chat-current-users').append(ich.ChatUsers({ ChatUsers: users }));
        }

    });

    return ChatCompositeView;
})