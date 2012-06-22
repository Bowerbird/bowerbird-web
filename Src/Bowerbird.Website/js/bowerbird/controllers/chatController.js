/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ChatController & ChatRouter
// ---------------------------

define(['jquery', 'underscore', 'backbone', 'app', 'models/chat', 'collections/usercollection',
'collections/chatmessagecollection', 'views/chatcompositeview', 'views/chatregion', 'signalr'],
function ($, _, Backbone, app, Chat, UserCollection, ChatMessageCollection, ChatCompositeView, ChatRegion) {

    var ChatRouter = function (options) {
        this.controller = options.controller;
        this.chatHub = $.connection.chatHub;
        this.userHub = $.connection.userHub;

        // Wire up hub callbacks
        this.chatHub.userIsTyping = userIsTyping;
        this.userHub.chatJoined = chatJoined;
        this.chatHub.userJoinedChat = userJoinedChat;
        this.chatHub.userExitedChat = userExitedChat;
        this.chatHub.chatMessageReceived = chatMessageReceived;
        this.chatHub.debugToLog = debugToLog;
    };

    var ChatController = {};

    var chatHub = $.connection.chatHub;

    // Shows a chat by creating a new region
    var showChat = function (chatView) {
        var chatRegion = new ChatRegion({ chat: chatView.model });
        app.chatRegions.push(chatRegion);
        chatRegion.show(chatView, 'append');
        return chatRegion;
    };

    // Used to generate an Guid for a private chat
    var generateGuid = function () {
        var S4 = function () {
            return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
        };
        return (S4() + S4() + "-" + S4() + "-" + S4() + "-" + S4() + "-" + S4() + S4() + S4());
    };

    // Hub Callbacks
    // -------------

    // Log info
    var debugToLog = function (message) {
        log(message);
    }

    // Chat joined
    var chatJoined = function (chatDetails) {
        var chatUsers = new UserCollection(chatDetails.Users);
        var chatMessages = new ChatMessageCollection(chatDetails.Messages);
        chat = new Chat({ Id: chatDetails.ChatId, Group: chatDetails.Group }, { chatUsers: chatUsers, chatMessages: chatMessages });
        app.chats.add(chat);
        var chatView = new ChatCompositeView({ id: 'chat-' + chatDetails.ChatId.replace(/\//g, '-'), model: chat });
        showChat(chatView);
    };

    // User joined chat
    var userJoinedChat = function (details) {
        var chat = app.chats.get(details.ChatId);
        chat.chatUsers.add(details.User);
    };

    var userExitedChat = function (details) {
        var chat = app.chats.get(details.ChatId);
        chat.chatUsers.remove(details.User.Id);
    };

    // Chat message received ready to display
    var chatMessageReceived = function (chatMessage) {
        // Find the chat
        var chat = app.chats.get(chatMessage.ChatId);

        // If this is a private chat, then this might be the first message we have received. If so, we need to create the chat
        // The server will have returned the user list with this initial message
        if (chat == null) {
            var chatUsers = new UserCollection(chatMessage.Users);
            var chatMessages = new ChatMessageCollection([chatMessage]);
            var chat = new Chat({ Id: chatMessage.ChatId, Group: chatMessage.Group }, { chatUsers: chatUsers, chatMessages: chatMessages });
            app.chats.add(chat);
            var chatView = new ChatCompositeView({ id: 'chat-' + chatMessage.ChatId.replace(/\//g, '-'), model: chat });
            showChat(chatView);
        } else {
            // Add the message
            chat.chatMessages.add(chatMessage);
        }
    };

    // User is typing a message status
    var userIsTyping = function (typingDetails) {
        log('user is typing', typingDetails);
        if (typingDetails.User.Id != app.authenticatedUser.user.id) {
            var chat = app.chats.get(typingDetails.ChatId);
            chat.chatUsers.get(typingDetails.User.Id).set('IsTyping', typingDetails.IsTyping);
        }
    };

    // ChatController Public API
    // -------------------------

    // Initiate a new private chat
    ChatController.startPrivateChat = function (user) {
        // Check to see if we have this user in a one-on-one private chat already
        var chat = app.chats.find(function (c) {
            return c.chatType() === 'private' && c.chatUsers.length == 2 && c.chatUsers.any(function (u) { return u.id === user.id }, this);
        }, this);
        if (app.authenticatedUser.user.id != user.id && !chat) {
            chatHub.startPrivateChat('chats/' + generateGuid(), [app.authenticatedUser.user.id, user.id]);
        }
    };

    // Join a group chat
    ChatController.joinGroupChat = function (group) {
        // Check to see if we have this user in a chat
        var chatId = 'chat/' + group.id;
        var chat = app.chats.find(function (c) { return c.id == chatId; }, this);
        if (!chat) {
            chatHub.joinChat(chatId, group.id);
        }
    };

    // Leave a chat
    ChatController.exitChat = function (chat) {
        chatHub.exitChat(chat.Id);
        app.chats.remove(chat.id);
        var chatRegion = _.find(app.chatRegions, function (region) { return region.chat.id === chat.id });
        chatRegion.close();
    };

    // Send typing status to other users
    ChatController.sendTypingStatus = function (chat, isTyping) {
        chatHub.typing(chat.id, isTyping);
    };

    // Send a chat message
    ChatController.sendChatMessage = function (chat, message) {
        chatHub.sendChatMessage(chat.id, message);
    };

    // ChatController Events
    // ---------------------

    app.vent.on('chats:joinGroupChat', function (group) {
        ChatController.joinGroupChat(group);
    });

    app.vent.on('chats:startPrivateChat', function (user) {
        ChatController.startPrivateChat(user);
    });

    app.vent.on('chats:useristyping', function (chat, isTyping) {
        ChatController.sendTypingStatus(chat, isTyping);
    });

    app.vent.on('chats:sendMessage', function (chat, message) {
        ChatController.sendChatMessage(chat, message);
    });

    app.vent.on('chats:close', function (chat) {
        ChatController.exitChat(chat);
    });
    //

    app.addInitializer(function () {
        this.chatRouter = new ChatRouter({
            controller: ChatController
        });
    });

    return ChatController;

});