
window.Bowerbird.ChatManager = Backbone.Model.extend({
    initialize: function (options) {
        console.log('chatManager.Initialize');
        _.extend(this, Backbone.Events);
        options.appManager.chats.on('add', this.showChatView, this);
        options.appManager.chats.on('remove', this.removeChatView, this);
        this.chatHub = $.connection.chatHub;
        this.chatHub.chatMessageReceived = this.chatMessageReceived;
        this.chatHub.chatUserStatusUpdate = this.chatUserStatusUpdate;
        this.chatHub.setupChat = this.setupChat;
        this.chatHub.typing = this.typing;
        //this.chatViews = [];
        console.log('chatManager.Initialize - done');
    },

//    showChatView: function (chat) {
//        console.log('appView.showChatView');
//        var chatView = new Bowerbird.Views.ChatView({ chat: chat, id: 'chat-' + chat.id });
//        this.chatViews.push(chatView);
//        $('body').append(chatView.render().el);
//    },

//    removeChatView: function (chat, options) {
//        var chatView = _.find(this.chatViews, function (item) {
//            return item.chat === chat;
//        });
//        chatView.remove();
//        this.chatViews = _.without(this.chatViews, chatView);
//    },

    startChat: function (chat) {
        console.log('chatManager.startChat');
        this.trigger('chatStarted', chat);
    },

    joinChat: function (chat) {
        this.chatHub.joinChat({ groupId: chat.get('group').get('id') });
    },

    exitChat: function (chat) {
        console.log('chatManager.exitChat');
        this.trigger('chatEnded', chat);
        this.chatHub.exitChat({ chatId: chat.id });
        // and the rest.... 
    },

    startTyping: function (chat) {
        console.log('chatManager.startTyping');
        this.chatHub.startTyping({ chatId: chat.id, typing: true });
    },

    stopTyping: function (chat) {
        console.log('chatManager.stopTyping');
        this.chatHub.startTyping({ chatId: chat.id, typing: false });
    },

    chatStatusUpdate: function (chat, status) {
        console.log('chatManager.chatStatusUpdate');
        this.chatHub.chatStatusUpdate({ chatId: chat.id, status: status });
    },

    setupChat: function (data) {
        // create the chat in the local DOM
        var chat = new Bowerbird.Models.Chat({ id: data.id });
        app.chats.add(chat);
        // grab the users from local DOM or add them
        // add chatUsers to chat
        // grab messages if there are any
    },

    typing: function (data) {
        // for the given data.chat
        // find the chatUser
        // set status to typing/not typing
    },

    sendMessage: function (message, chat) {
        console.log('chatManager.sendMessage');
        this.chatHub.sendChatMessage(app.get('userId'), chat.get('group').get('id'), message);
    },

    chatMessageReceived: function (data) {
        console.log('chatManager.chatMessageReceived');
        console.log('message for groupId: ' + data.groupId + ' with content ' + data.message);
        var chat = app.chats.get(data.groupId);
        chat.chatMessages.add(new Bowerbird.Models.ChatMessage(data));
    },

    chatUserStatusUpdate: function (data) {
        console.log('chatManager.chatUserStatusUpdate');
        var chat = app.chats.get(data.id);
        var chatUser = _.find(chat.chatUsers, function (item) {
            return item.user === data.user;
        });
        if (_.isNull(chatUser) || _.isUndefined(chatUser)) {
            if (data.status == 2 || data.status == 3 || data.status == 'undefined') return;
            var user = app.users.get(data.user.id);
            if (_.isNull(user) || _.isUndefined(user)) {
                user = new Bowerbird.Models.User(data.user);
            }
            chatUser = new Bowerbird.Models.ChatUser({ chat: chat, user: user, status: data.status });
            chat.chatUsers.add(chatUser);
        } else {
            if (data.status == 2 || data.status == 3) {
                chatUser.remove();
            }
            else {
                chatUser.set('status', data.status);
            }
        }
    }
});