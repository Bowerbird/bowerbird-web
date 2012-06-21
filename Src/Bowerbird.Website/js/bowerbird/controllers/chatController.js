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
        this.hub = $.connection.chatHub;
        this.controller = options.controller;

        groupChatJoined
        userJoinedChat
        userIsTyping
        chatMessageReceived

        this.hub.chatMessageReceived = chatMessageReceived;
        this.hub.userJoinedChat = userJoinedChat;
        this.hub.groupChatJoined = groupChatJoined;
        this.hub.userExitedChat = userExitedChat;
        this.hub.userIsTyping = userIsTyping;
        this.hub.debugToLog = debugToLog;
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

    // Group chat successfully joined
    var groupChatJoined = function (data) {
        var chat = app.chats.get(data.ChatId);
        chat.chatUsers.add(data.Users);
        chat.chatMessages.add(data.Messages);
        var chatView = new ChatCompositeView({ id: 'chat-' + chat.id, model: chat });
        showChat(chatView);
    };

    // User joined chat
    var userJoinedChat = function (data) {

    };

    var setupOnlineUsers = function (onlineUsers) {
        
    };

    // Chat message received ready to display
    var chatMessageReceived = function (data) {
        // Find the chat
        var chat = app.chats.get(data.ChatId);

        if (chat == null) {
            var chatUsers = new UserCollection(data.Users);
            var chatMessages = new ChatMessageCollection();
            var chat = new Chat({ Id: data.ChatId }, { chatUsers: chatUsers, chatMessages: chatMessages });
            app.chats.add(chat);
            var chatView = new ChatCompositeView({ id: 'chat-' + data.ChatId, model: chat });
            showChat(chatView);
        }

        // Add the message
        chat.chatMessages.add(data);
    };

    // User is typing a message status
    var userIsTyping = function (data) {

    };

    // User left a chat status
    var userExitedChat = function (data) {

    };

    // ChatController Public API
    // -------------------------

    // Initiate a new private chat
    ChatController.startNewPrivateChat = function (user) {
        // Check to see if we have this user in a one-on-one private chat already
        var chatExists = app.chats.any(function (c) {
            return c.chatType() === 'private' && c.chatUsers.length == 2 && c.chatUsers.any(function (u) { return u.id === user.id }, this);
        }, this);
        if (app.authenticatedUser.user.id != user.id && !chatExists) {
            var chatId = generateGuid();
            var chatUsers = new UserCollection([app.authenticatedUser.user, user]);
            var chatMessages = new ChatMessageCollection();
            var chat = new Chat({ Id: chatId, User: user }, { chatUsers: chatUsers, chatMessages: chatMessages });
            app.chats.add(chat);
            var chatView = new ChatCompositeView({ id: 'chat-' + chatId, model: chat });
            showChat(chatView);
        }
    };

    // Join a group chat
    ChatController.joinGroupChat = function (group) {
        // Check to see if we have this user in a chat
        var chatExists = app.chats.any(function (c) { return c.id == group.id; }, this);
        if (!chatExists) {
            var chatUsers = new UserCollection();
            var chatMessages = new ChatMessageCollection();
            var chat = new Chat({ Id: group.id, Group: group }, { chatUsers: chatUsers, chatMessages: chatMessages });
            app.chats.add(chat);
            // Subscribe user to chat
            chatHub.joinChat(group.id);
        }
    };

    // Leave a chat
    ChatController.exitChat = function (chat) {
        chatHub.exitChat(chat.Id);
    };

    // Send typing status to other users
    ChatController.sendTypingStatus = function (id, isTyping) {
        log('chatRouter.typing');
        chatHub.typing(id, isTyping);
    };

    // Send a chat message
    ChatController.sendChatMessage = function (chat, message) {
        chatHub.sendChatMessage(chat.id, message, chat.chatUsers.pluck('Id'));
    };

    // ChatController Events
    // ---------------------

    app.vent.on('chats:joinGroupChat', function (group) {
        ChatController.joinGroupChat(group);
    });

    app.vent.on('chats:startPrivateChat', function (user) {
        ChatController.startNewPrivateChat(user);
    });

    app.vent.on('chats:sendMessage', function (chat, message) {
        ChatController.sendChatMessage(chat, message);
    });

    app.vent.on('chats:close', function (chat) {
        app.chats.remove(chat.id);
        var chatRegion = _.find(app.chatRegions, function (region) { return region.chat.id === chat.id });
        chatRegion.close();
    });

    app.addInitializer(function () {
        this.chatRouter = new ChatRouter({
            controller: ChatController
        });
    });

    return ChatController;

});