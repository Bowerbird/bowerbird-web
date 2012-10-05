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

        // Wire up user hub callbacks
        this.userHub.chatJoined = chatJoined;
        this.userHub.chatExited = chatExited;

        // Wire up chat hub callbacks
        this.chatHub.userIsTyping = userIsTyping;
        this.chatHub.userJoinedChat = userJoinedChat;
        this.chatHub.userExitedChat = userExitedChat;
        this.chatHub.newChatMessage = newChatMessage;
        this.chatHub.debugToLog = debugToLog;

        this.getChat = function (chatId) {
            return this.chatHub.getChat(chatId);
        };

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

    // Close chat window
    var closeChat = function (chat) {
        app.chats.remove(chat.id);
        var chatRegion = _.find(app.chatRegions, function (region) { return region.chat.id === chat.id; });
        app.chatRegions.splice(_.indexOf(app.chatRegions, chatRegion), 1);
        chatRegion.close();
    };

    var generateHash = function (value) {
        var hash = 0;
        if (value.length == 0) return hash;
        for (var i = 0; i < value.length; i++) {
            var char = value.charCodeAt(i);
            hash = ((hash << 5) - hash) + char;
            hash = hash & hash; // Convert to 32bit integer
        }
        return hash;
    };

    // Generate chat id based of of ids
    var generateChatId = function (ids) {
        var sortedIds = _.sortBy(ids, function (name) { return name; });
        return 'chats/' + generateHash(sortedIds.join(''));
    };

    // Hub Callbacks
    // -------------

    // Log info
    var debugToLog = function (message) {
        log(message);
    };

    // Chat joined
    var chatJoined = function (chatDetails) {
        var chat = app.chats.get(chatDetails.ChatId);

        // Chat might not exist if this is a group chat and the user has not received any messages on a secondary browser session
        if (chat == null) {
            chat = showChat(chatDetails.ChatId, chatDetails.Users, chatDetails.Messages, chatDetails.Group);
        }
        if (chat.get('IsStarted') === false) {
            chat.start(chatDetails);
        }
    };

    // Chat exited
    var chatExited = function (chatId) {
        // This callback gets fired so that we can close other browser session windows, if the user has more than one open
        var chat = app.chats.get(chatId);
        if (chat) {
            closeChat(chat);
        }
    };

    // User joined group chat 
    var userJoinedChat = function (details) {
        var chat = app.chats.get(details.ChatId);
        chat.chatUsers.add(details.FromUser);
        // Only add other users' joining messages
        if (details.FromUser.Id !== app.authenticatedUser.user.id && chat.chatType() === 'group') {
            chat.chatMessages.add(details);
        }
    };

    // User existed group chat
    var userExitedChat = function (details) {
        var chat = app.chats.get(details.ChatId);
        chat.chatUsers.remove(chat.chatUsers.get(details.FromUser.Id));
        // Only add other users' leaving messages
        if (details.FromUser.Id !== app.authenticatedUser.user.id && chat.chatType() === 'group') {
            chat.chatMessages.add(details);
        }
    };

    // Chat message received ready to display
    var newChatMessage = function (chatMessage) {
        log('new chat message:', chatMessage);
        // Find the chat
        var chat = app.chats.get(chatMessage.ChatId);

        // If this is a private chat, then this might be the first message we have received. If so, we need to create the chat
        if (chat == null) {
            app.chatRouter.getChat(chatMessage.ChatId)
                .done(function (chatDetails) {
                    log('chat received', chatDetails);
                    chat = showChat(chatMessage.ChatId, chatDetails.Users, chatDetails.Group ? chatDetails.Messages : [chatMessage], chatDetails.Group);
                    chat.start();
                })
                .fail(function (error) {
                    // TODO: Error handling
                });
        } else {
            // Add the message
            var existingChatMessage = chat.chatMessages.get(chatMessage.Id);
            if (existingChatMessage) {
                // If the chat message exists, we just update its details (this will show the datetime of the message being uploaded
                existingChatMessage.set(chatMessage);
            } else {
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
        var chatId = generateChatId([app.authenticatedUser.user.id, user.id]);
        var chat = app.chats.find(function (c) { return c.id == chatId; }, this);
        if (app.authenticatedUser.user.id != user.id && !chat) { // can't chat with self!
            showChat(chatId, [user], [], null);
            app.chatRouter.joinChat(chatId, [app.authenticatedUser.user.id, user.id], null);
        }
    };

    // Join a group chat
    ChatController.joinGroupChat = function (group) {
        // Check to see if we have this user in a chat
        var chatId = generateChatId([group.id]);
        var chat = app.chats.find(function (c) { return c.id == chatId; }, this);
        if (!chat) {
            showChat(chatId, [app.authenticatedUser.user], [], group.toJSON());
            app.chatRouter.joinChat(chatId, [app.authenticatedUser.user.id], group.id);
        //} else {
        //    showChat(chatId, [app.authenticatedUser.user], [], group.toJSON());
        }
    };

    // Leave a chat
    ChatController.exitChat = function (chat) {
        if (chat.chatType() === 'group') {
            app.chatRouter.exitChat(chat.id);
        }
        closeChat(chat);
    };

    // Send typing status to other users
    ChatController.sendTypingStatus = function (chat, isTyping) {
        app.chatRouter.typing(chat.id, isTyping);
    };

    // Send a chat message
    ChatController.sendChatMessage = function (chat, message) {
        // Add the chat message to the message list so that user sees it instantly
        var messageId = app.generateGuid();
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