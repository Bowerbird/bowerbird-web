
window.Bowerbird.Views.ChatView = Backbone.View.extend({
    className: 'chat-window',

    events: {
        "click .chat-send-message-button": "sendMessage",
        "click .window-close": "closeWindow"
    },

    template: $.template('chatTemplate', $('#chat-template')),

    chatMessageTemplate: $.template('chatMessageTemplate', $('#chat-message-template')),

    chatViewUserTemplate: $.template('chatViewUserTemplate', $('#chat-user-template')),

    initialize: function (options) {
        _.extend(this, Backbone.Events);
        _.bindAll(this, 'addChatViewUser', 'addChatViewUsers');
        this.chatViewUsers = [];
        this.chat = options.chat;
        this.chat.chatMessages.on('add', this.addChatMessage, this);
        this.chat.chatUsers.on('add', this.addChatViewUser, this);
        this.chat.chatUsers.on('reset', this.addChatViewUsers, this);

    },

    render: function () {
        $.tmpl('chatTemplate', this.chat.toJSON()).appendTo(this.$el);
        return this;
    },

    sendMessage: function () {
        console.log('chatView.sendMessage');
        app.chatManager.sendMessage(this.$el.find('.new-chat-message').val(), this.chat);
        //console.log('send message!');
    },

    closeWindow: function () {
        app.chats.remove(this.chat);
        //this.$el.remove();
    },

    addChatMessage: function (chatMessage) {
        console.log('chatView.addChatMessage');
        $.tmpl('chatMessageTemplate', chatMessage.toJSON()).appendTo(this.$el.find('.chat-messages'));
    },

    addChatViewUser: function (chatUser) {
        console.log('chatView.addChatViewUser');
        var chatViewUser = new Bowerbird.Views.ChatViewUser({ user: chatUser });
        this.chatViewUsers.push(chatViewUser);
        //this.$el.find('.chat-current-users').append(chatViewUser.render().el);
        $.tmpl('chatViewUserTemplate', chatUser.toJSON()).appendTo(this.$el.find('.chat-current-users'));
    },

    addChatViewUsers: function (chatUsers) {
        console.log('chatView.addChatViewUsers');
        var self = this;
        chatUsers.each(function (chatUser) {
            self.addChatViewUser(chatUser);
        });
        //        var chatViewUser = new Bowerbird.Views.ChatViewUser({ user: chatUser });
        //        this.chatViewUsers.push(chatUser);
        //        this.$el.find('.chat-current-users').append(chatViewUser.render().el);
    }
});