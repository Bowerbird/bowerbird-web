/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ChatController & ChatRouter
// ---------------------------

define(['jquery', 'underscore', 'backbone', 'app', 'models/chat'],
function ($, _, Backbone, app, Chat) {

    var ChatRouter = function (options) {
        this.hub = $.connection.chatHub;
        this.controller = options.controller;

        this.hub.startChat = this.controller.startChat;
        this.hub.chatMessageReceived = this.controller.receiveMessage;
        this.hub.typing = this.controller.typing;
        this.hub.userExitedChat = this.controller.userExitedChat;
        this.hub.chatRequest = this.controller.chatRequest;
        this.hub.userJoinedChat = this.controller.userJoinedChat;
        this.hub.setupChat = this.controller.setupChat;
    };

    var ChatController = {};


    // ChatController Public API To HUB
    //---------------------------------

    app.vent.on('chats:startGroupChat', function (id) {
        log('chats.startGroupChat with group: ' + id);
        //GroupUserController.showHome(id);
    });

    app.vent.on('chats:startPrivateChat', function (id) {
        log('chats.startPrivateChat with user: ' + id);
        //GroupUserController.showHome(id);
    });

    ChatController.joinChat = function (chat) {
        if (chat.get('Type') == "Group"){// instanceof Bowerbird.Models.GroupChat) {
            this.chatHub.joinChat(chat.get('Group').get('Id'));
        }
        else if (chat.get('Type') == "Private"){ //instanceof Bowerbird.Models.UserChat) {
            this.chatHub.startChat(chat.Id, chat.get('User').get('Id'));
        }
    };

    ChatController.exitChat = function (chat) {
        log('chatRouter.exitChat');
        //this.trigger('chatEnded', chat);
        this.chatHub.exitChat(chat.Id);
        // and the rest.... 
    };

    ChatController.startTyping = function (chat) {
        log('chatRouter.startTyping');
        this.chatHub.typing(chat.Id, true);
    };

    ChatController.stopTyping = function (chat) {
        log('chatRouter.stopTyping');
        this.chatHub.typing(chat.Id, false);
    };

    ChatController.sendMessage = function (message, chat) {
        log('chatRouter.sendMessage');
        this.chatHub.sendChatMessage(chat.get('Id'), message);
    };

    ChatController.inviteToChat = function (chat, user) {
        log('chatRouter.inviteToChat: invited ' + user.get('Name') + ' to chat');
        this.chatHub.inviteToChat(chat.get('Group').get('Id'), user.Id);
    };


    // ChatController Public API From HUB
    // ----------------------------------

    ChatController.startChat = function (data) {
        log('chatController.startChat', this, data);
        //app.activities.add(data);
    };

    ChatController.chatMessageReceived = function (data) {
        log('chatController.chatMessageReceived');
    };

    ChatController.typing = function (data) {
        log('chatController.typing');
    };

    ChatController.userExitedChat = function (data) {
        log('chatController.userExitedChat');
    };

    ChatController.chatRequest = function (data) {
        log('chatController.chatRequest');
    };

    ChatController.userJoinedChat = function (data) {
        log('chatController.userJoinedChat');
    };

    app.addInitializer(function () {
        this.chatRouter = new ChatRouter({
            controller: ChatController
        });
    });

    return ChatController;

});