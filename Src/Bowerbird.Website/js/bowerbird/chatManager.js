
window.Bowerbird.ChatManager = Backbone.Model.extend({
    initialize: function (options) {
        _.extend(this, Backbone.Events);
        _.bindAll(this, 'connectedUsers');
        options.appManager.chats.on('add', this.startChat, this);
        this.chatHub = $.connection.chatHub;
        this.chatHub.chatMessageReceived = this.chatMessageReceived;
        this.chatHub.connectedUsers = this.connectedUsers;
    },

    startChat: function (chat) {
        console.log('chatManager.startChat');
        this.trigger('chatStarted', chat);
        this.chatHub.getConnectedChatUsers(chat.get('group').get('id'));
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

    connectedUsers: function (data) {
        console.log('chatManager.connectedUsers');
        console.log('group: ' + data.groupId);
        var chat = app.chats.get(data.groupId);
        var users = data.connectedUsers;
        chat.chatUsers.reset(users);
//        console.log('connected users: ' + users.length);
//        $.each(users, function (i,user) {
//            console.log('adding user: ' + user.id + ' ' + user.name);
//            chat.chatUsers.add(user);
//        });
    }
});