
window.Bowerbird.ChatRouter = Backbone.Model.extend({

    // INIT-----------------------------------------

    initialize: function (options) {

        console.log('chatRouter.Initialize');
        _.extend(this, Backbone.Events);
        _.bindAll(this, 'setupChat');

        this.appManager = options.appManager;
        //this.appView = this.appManager.get('appView');
        //this.appManager.chats.on('add', this.appView.showChatView, this);
        //this.appManager.chats.on('remove', this.appView.removeChatView, this);

        this.chatHub = $.connection.chatHub;
        this.chatHub.chatMessageReceived = this.chatMessageReceived;
        this.chatHub.chatUserStatusUpdate = this.chatUserStatusUpdate;
        this.chatHub.setupChat = this.setupChat;
        this.chatHub.typing = this.typing;

        console.log('chatRouter.Initialize - done');
    },

    //    startChat: function (chat) {
    //        console.log('chatRouter.startChat');
    //        this.trigger('chatStarted', chat);
    //    },

    showChatView: function () {
        alert('hi fool');
    },


    // TO HUB---------------------------------------

    joinChat: function (chat) {
        //var chatId = chat.get('group').get('id');
        //var type = chatId.split('/')[0];
        //var id = chatId.split('/')[1];
        //this.chatHub.joinChat({ chatId: chat.get('group').get('id') });
        this.chatHub.joinChat(chat.get('group').get('id'));
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

    sendMessage: function (message, chat) {
        console.log('chatManager.sendMessage');
        this.chatHub.sendChatMessage(this.appManager.get('userId'), chat.get('group').get('id'), message);
    },


    // FROM HUB-------------------------------------

    setupChat: function (data) {
        console.log('chatRouter.setupChat');
        var self = this;
        var chat = self.appManager.chats.get(data.id);
        var chatUser = null;
        $.each(data.users, function (index, xitem) {
            chatUser = _.find(chat.chatUsers, function (yitem) {
                return xitem.user === yitem.user;
            });
            if (_.isNull(chatUser) || _.isUndefined(chatUser)) {
                var user = self.appManager.users.get(xitem.id);
                if (_.isNull(user) || _.isUndefined(user)) {
                    user = new Bowerbird.Models.User(xitem);
                }
                chatUser = new Bowerbird.Models.ChatUser({ chat: chat, user: user, status: 0 });
            }
            chat.chatUsers.add(chatUser);
        });
    },

    typing: function (data) {
        // for the given data.chat
        // find the chatUser
        // set status to typing/not typing
    },

    chatMessageReceived: function (data) {
        console.log('chatManager.chatMessageReceived');
        console.log('message for groupId: ' + data.groupId + ' with content ' + data.message);
        var chat = this.appManager.chats.get(data.groupId);
        chat.chatMessages.add(new Bowerbird.Models.ChatMessage(data));
    },

    chatUserStatusUpdate: function (data) {
        console.log('chatManager.chatUserStatusUpdate');
        var chat = this.appManager.chats.get(data.id);
        var chatUser = _.find(chat.chatUsers, function (item) {
            return item.user === data.user;
        });
        if (_.isNull(chatUser) || _.isUndefined(chatUser)) {
            if (data.status == 2 || data.status == 3 || data.status == 'undefined') return;
            var user = this.appManager.users.get(data.user.id);
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