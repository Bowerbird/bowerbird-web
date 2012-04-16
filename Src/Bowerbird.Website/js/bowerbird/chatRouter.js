
window.Bowerbird.ChatRouter = Backbone.Model.extend({

    // INIT-----------------------------------------

    initialize: function (options) {

        log('chatRouter.Initialize');

        _.extend(this, Backbone.Events);
        _.bindAll(this,
        'joinChat',
        'exitChat',
        'startTyping',
        'stopTyping',
        'sendMessage',
        'inviteToChat',
        'setupChat',
        'typing',
        'chatMessageReceived',
        'userJoinedChat',
        'userExitedChat',
        'chatRequest'
        );

        app.chats.on('remove', this.exitChat, this);
        this.chatHub = $.connection.chatHub;
        this.chatHub.chatMessageReceived = this.chatMessageReceived;
        this.chatHub.userJoinedChat = this.userJoinedChat;
        this.chatHub.userExitedChat = this.userExitedChat;
        this.chatHub.setupChat = this.setupChat;
        this.chatHub.typing = this.typing;
        this.chatHub.chatRequest = this.chatRequest;

        log('chatRouter.Initialize - done');
    },

    // TO HUB---------------------------------------

    joinChat: function (chat) {
        if (chat instanceof Bowerbird.Models.GroupChat) {
            this.chatHub.joinChat(chat.get('Group').get('Id'));
        }
        else if (chat instanceof Bowerbird.Models.UserChat) {
            this.chatHub.startChat(chat.Id, chat.get('User').get('Id'));
        }
    },

    exitChat: function (chat) {
        log('chatRouter.exitChat');
        //this.trigger('chatEnded', chat);
        this.chatHub.exitChat(chat.Id);
        // and the rest.... 
    },

    startTyping: function (chat) {
        log('chatRouter.startTyping');
        this.chatHub.typing(chat.Id, true);
    },

    stopTyping: function (chat) {
        log('chatRouter.stopTyping');
        this.chatHub.typing(chat.Id, false);
    },

    sendMessage: function (message, chat) {
        log('chatRouter.sendMessage');
        this.chatHub.sendChatMessage(chat.get('Id'), message);
    },

    inviteToChat: function (chat, user) {
        log('chatRouter.inviteToChat: invited ' + user.get('Name') + ' to chat');
        this.chatHub.inviteToChat(chat.get('Group').get('Id'), user.Id);
    },

    // FROM HUB-------------------------------------

    setupChat: function (data) {
        log('chatRouter.setupChat');
        var self = this;
        var chat = app.chats.get(data.Id);

        $.each(data.Users, function (index, xitem) {
            var chatUser = _.find(chat.ChatUsers, function (yitem) {
                return xitem.User.Id == yitem.User.Id;
            });
            if (_.isNull(chatUser) || _.isUndefined(chatUser)) {
                var user = app.onlineUsers.get(xitem.Id);
                if (_.isNull(user) || _.isUndefined(user)) {
                    user = new Bowerbird.Models.User(xitem);
                }
                chatUser = new Bowerbird.Models.ChatUser({ Chat: chat, User: user, Status: 0 });
            }
            chat.ChatUsers.add(chatUser);
        });
    },

    typing: function (data) {
        // for the given data.chat
        // find the chatUser
        // set status to typing/not typing
    },

    chatMessageReceived: function (data) {
        log('message for chatId: ' + data.ChatId + ' with content ' + data.Message);
        var chat = app.chats.get(data.ChatId);
        chat.ChatMessages.add(new Bowerbird.Models.ChatMessage(data));
    },

    userJoinedChat: function (data) {
        log('chatRouter.userJoinedChat');
        var chat = app.chats.get(data.Id);

        var chatUsers = chat.ChatUsers.pluck('User');

        var match = _.any(chatUsers, function (user) {
            return user.Id == data.User.Id;
        });

        if (match) return;

        var user = app.onlineUsers.get(data.User.Id);
        if (_.isNull(user) || _.isUndefined(user)) {
            user = new Bowerbird.Models.User(data.User);
        }
        chatUser = new Bowerbird.Models.ChatUser({ Chat: chat, User: user, Status: data.Status });
        chat.ChatUsers.add(chatUser);
    },

    userExitedChat: function (data) {
        log('chatRouter.userExitedChat');
        var chat = app.Chats.get(data.Id);

        var chatUsers = chat.ChatUsers.pluck('user');
        var match = _.any(chatUsers, function (user) {
            return user.Id == data.User.Id;
        });

        if (!(match)) return;

        var chatUser = null;
        chat.ChatUsers.each(function(item){
            if (item.get('User').Id == data.User.Id) {
                chatUser = item;
            }
        });

        if (_.isNull(chatUser) || _.isUndefined(chatUser)) {
            return;
        } else {
            chat.ChatUsers.remove(chatUser);
        }
    },

    chatRequest: function (data) {
        log('chatRouter.chatRequest');
        var fromUser = app.onlineUsers.get(data.FromUser.Id);
        var chatId = data.ChatId;
        log('>> ' + data.FromUser.Name + ' says "' + data.Message + '" @' + data.Timestamp);
        var chat = new Bowerbird.Models.UserChat({ Id: chatId, User: fromUser, Message: data.message, Timestamp: data.timestamp });
        app.Chats.add(chat);
        // add chat message at this point>?
        this.joinChat(chat);
    }

});