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
        this.chatHub = options.chatHub;
        this.userHub = options.userHub;

        // Wire up hub callbacks
        this.chatHub.userIsTyping = userIsTyping;
        this.userHub.chatJoined = chatJoined;
        this.chatHub.userJoinedChat = userJoinedChat;
        this.chatHub.userExitedChat = userExitedChat;
        this.chatHub.newChatMessage = newChatMessage;
        this.chatHub.debugToLog = debugToLog;

        this.joinChat = function (chatId, userIds, groupId) {
            this.chatHub.joinChat(chatId, userIds, groupId);
        };

        this.exitChat = function (chatId) {
            this.chatHub.exitChat(chatId);
        };

        this.typing = function (chatId, isTyping) {
            this.chatHub.typing(chatId, isTyping);
        };

        this.sendChatMessage = function (chatId, messageId, message) {
            this.chatHub.sendChatMessage(chatId, messageId, message);
        };
    };

    // Shows a chat by creating a model, view and adding it to a new region
    var showChat = function (chatId, users, messages, group) {
        var chatUsers = new UserCollection(users);
        var chatMessages = new ChatMessageCollection(messages);
        var chat = new Chat({ Id: chatId, Group: group }, { chatUsers: chatUsers, chatMessages: chatMessages });
        app.chats.add(chat);
        var chatView = new ChatCompositeView({ id: 'chat-' + chatId.replace(/\//g, '-'), model: chat });
        var chatRegion = new ChatRegion({ chat: chat });
        app.chatRegions.push(chatRegion);
        chatRegion.show(chatView, 'append');
        return chat;
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
        var chat = app.chats.get(chatDetails.ChatId);
        chat.start(chatDetails);
    };

    // User joined chat
    var userJoinedChat = function (details) {
        var chat = app.chats.get(details.ChatId);
        chat.chatUsers.add(details.FromUser);
        chat.chatMessages.add(details);
    };

    // User existed chat
    var userExitedChat = function (details) {
        var chat = app.chats.get(details.ChatId);
        chat.chatUsers.remove(details.User.Id);
        chat.chatMessages.add(details);
    };

    // Chat message received ready to display
    var newChatMessage = function (chatMessage) {
        // Find the chat
        var chat = app.chats.get(chatMessage.ChatId);

        // If this is a private chat, then this might be the first message we have received. If so, we need to create the chat
        // The server will have returned the user list with this initial message
        if (chat == null) {
            chat = showChat(chatMessage.ChatId, chatMessage.Users, [chatMessage], chatMessage.Group);
            chat.start();
        } else {
            // Add the message
            var existingChatMessage = chat.chatMessages.get(chatMessage.Id);
            if (existingChatMessage) {
                existingChatMessage.set(chatMessage);
            }
            else {
                chat.chatMessages.add(chatMessage);
            }
        }
    };

    // User is typing a message status
    var userIsTyping = function (typingDetails) {
        var chat = app.chats.get(typingDetails.ChatId);
        chat.chatUsers.get(typingDetails.UserId).set('IsTyping', typingDetails.IsTyping);
    };

    // ChatController Public API
    // -------------------------

    var ChatController = {};

    // Initiate a new private chat
    ChatController.startPrivateChat = function (user) {
        // Check to see if we have this user in a one-on-one private chat already
        var chat = app.chats.find(function (c) {
            return c.chatType() === 'private' && c.chatUsers.length == 2 && c.chatUsers.any(function (u) { return u.id === user.id }, this);
        }, this);
        if (app.authenticatedUser.user.id != user.id && !chat) {
            var chatId = 'chats/' + generateGuid();
            showChat(chatId, [user], [], null);
            app.chatRouter.joinChat(chatId, [app.authenticatedUser.user.id, user.id], null);
        }
    };

    // Join a group chat
    ChatController.joinGroupChat = function (group) {
        // Check to see if we have this user in a chat
        var chatId = 'chats/' + group.id;
        var chat = app.chats.find(function (c) { return c.id == chatId; }, this);
        if (!chat) {
            showChat(chatId, [app.authenticatedUser.user], [], group);
            app.chatRouter.joinChat(chatId, [app.authenticatedUser.user.id], group.id);
        }
    };

    // Leave a chat
    ChatController.exitChat = function (chat) {
        app.chatRouter.exitChat(chat.id);
        app.chats.remove(chat.id);
        var chatRegion = _.find(app.chatRegions, function (region) { return region.chat.id === chat.id });
        chatRegion.close();
    };

    // Send typing status to other users
    ChatController.sendTypingStatus = function (chat, isTyping) {
        app.chatRouter.typing(chat.id, isTyping);
    };

    // Send a chat message
    ChatController.sendChatMessage = function (chat, message) {
        // Add the chat message to the message list so that user sees it instantly
        var messageId = generateGuid();
        chat.chatMessages.add({ Id: messageId, Type: 'usermessage', ChatId: chat.id, FromUser: app.authenticatedUser.user.toJSON(), Timestamp: '', Message: message });
        app.chatRouter.sendChatMessage(chat.id, messageId, message);
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

    app.addInitializer(function () {
        this.chatRouter = new ChatRouter({
            chatHub: $.connection.chatHub,
            userHub: $.connection.userHub
        });
    });

    return ChatController;

});