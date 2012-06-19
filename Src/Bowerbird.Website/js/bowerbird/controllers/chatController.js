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

        this.hub.chatMessageReceived = this.controller.chatMessageReceived;
        this.hub.typing = this.controller.typing;
        this.hub.userExitedChat = this.controller.userExitedChat;
        this.hub.chatRequest = this.controller.chatRequest;
        this.hub.userJoinedChat = this.controller.userJoinedChat;
        this.hub.setupChat = this.controller.setupChat;

        this.hub.debugToLog = this.controller.debugToLog;
    };

    var ChatController = {};

    ChatController.hub = $.connection.chatHub;

    var showChat = function (chatView) {
        var chatRegion = new ChatRegion({ chat: chatView.model });
        app.chatRegions.push(chatRegion);
        chatRegion.show(chatView, 'append');
        return chatRegion;
    };

    // ChatController Public API To HUB
    //---------------------------------

    app.vent.on('chats:startGroupChat', function (id) {
        log('chats.startGroupChat with group: ' + id);
        ChatController.joinChat(id);
    });

    app.vent.on('chats:startPrivateChat', function (id) {
        log('chats.startPrivateChat with user: ' + id);
        ChatController.joinChat(id);
    });

    app.vent.on('chats:sendMessage', function (chatId, message) {
        //log('chatController:  chatId, message);
        ChatController.sendMessage(chatId, message);
    });

    ChatController.debugToLog = function (message) {
        log(message);
    }

    // join a group/private chat
    ChatController.joinChat = function (id) {
        log('chatController.joinChat: with ' + id);
        if (app.chats.all(function (item) { return item.id != id }, this)) {
            if (id.split('/')[0] == 'users') {// instanceof Bowerbird.Models.GroupChat) {
                var chatId = ChatController.generateGuid();
                var userId = id;
                ChatController.hub.startChat(chatId, userId);
            }
            else { //instanceof Bowerbird.Models.UserChat) {
                ChatController.hub.joinChat(id);
            }
        }
    };

    // leave a chat
    ChatController.exitChat = function (chat) {
        log('chatRouter.exitChat');
        //this.trigger('chatEnded', chat);
        // and the rest.... 
        this.hub.exitChat(chat.Id);
    };

    // toggle typing
    ChatController.typing = function (id, isTyping) {
        log('chatRouter.typing');
        this.hub.typing(id, isTyping);
    };

    // send a chat message
    ChatController.sendMessage = function (chatId, message) {
        log('chatController.sendMessage message:' + message + ' to: ' + chatId);
        this.hub.sendChatMessage(chatId, message);
    };

    // ChatController Public API From HUB
    // ----------------------------------

    // add the chat to the chats collection
    // create and show a new chat window
    // push the list of users and messages into the chat
    // data object has properties:, Title, Timestamp, Users[], Messages[]
    ChatController.setupChat = function (data) {
        log('chatController.setupChat', this, data);

        var chatUsers = new UserCollection(data.Users);
        var chatMessages = new ChatMessageCollection(data.Messages);
        var chat = new Chat({ Id: data.ChatId, Title: data.Title }, { chatUsers: chatUsers, chatMessages: chatMessages });

        app.chats.add(chat);
        var chatView = new ChatCompositeView({ id: 'chat-' + data.ChatId, model: chat });
        log(chatView);

        showChat(chatView);
    };

    // create a new private chat
    ChatController.chatRequest = function (data) {
        log('chatController.chatRequest', this, data);

        var chatUsers = new UserCollection(data.Users);
        var chatMessages = new ChatMessageCollection(data.Messages);
        var chat = new Chat({ Id: data.ChatId, Title: data.Title }, { chatUsers: chatUsers, chatMessages: chatMessages });

        app.chats.add(chat);
        var chatView = new ChatCompositeView({ id: 'chat-' + data.ChatId, model: chat });
        log(chatView);

        showChat(chatView);
    };

    // grab the chat from the chats collection
    // if the user is in the chat, ignore otherwise add
    ChatController.userJoinedChat = function (data) {
        log('chatController.userJoinedChat', this, data);
    };

    // find the chat
    // pop the message in the chat's message collection
    ChatController.chatMessageReceived = function (data) {
        log('chatController.chatMessageReceived', this, data);

        // find the chat
        var chat = app.chats.get(data.ChatId);


        //chat.addChatMessage(data);
        // add the message
        chat.chatMessages.add(data);

        //app.vent.trigger('newmessage:' + data.ChatId);
    };

    // change typing icon for this chat
    ChatController.typing = function (data) {
        log('chatController.typing', this, data);
    };

    // grab the chat from the chats collection
    // if the user is in the chat, remove them
    ChatController.userExitedChat = function (data) {
        log('chatController.userExitedChat', this, data);
    };

    // used to generate an Guid for a private chat
    ChatController.generateGuid = function () {
        var S4 = function () {
            return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
        };
        return (S4() + S4() + "-" + S4() + "-" + S4() + "-" + S4() + "-" + S4() + S4() + S4());
    };

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