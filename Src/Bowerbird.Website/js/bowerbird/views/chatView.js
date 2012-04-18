
window.Bowerbird.Views.ChatView = Backbone.View.extend({
    className: 'chat-window',

    events: {
        "click .chat-send-message-button": "sendMessage",
        "click .window-close": "closeWindow"
    },

    initialize: function (options) {
        _.extend(this, Backbone.Events);
        _.bindAll(this,
        'sendMessage',
        'closeWindow',
        'addChatMessage',
        'changeUsers');
        this.chat = options.chat;
        this.chat.chatUsers.on('add', this.changeUsers, this);
        this.chat.chatUsers.on('remove', this.changeUsers, this);
        this.chat.chatUsers.on('reset', this.changeUsers, this);
        this.chat.chatMessages.on('add', this.addChatMessage, this);
    },

    render: function () {
        this.$el.append(ich.ChatWindow({ Title: this.chat.get('Title'), Messages: this.chat.chatMessages.toJSON(), ChatUsers: this.chat.chatUsers.toJSON()}));
        return this;
    },

    sendMessage: function () {
        log('chatView.sendMessage');
        app.chatRouter.sendMessage(this.$el.find('.new-chat-message').val(), this.Chat);
        this.$el.find('.new-chat-message').val('');
    },

    closeWindow: function () {
        app.chats.remove(this.Chat);
        this.$el.remove();
    },

    addChatMessage: function (chatMessage) {
        log('chatView.addChatMessage');
        this.$el.find('.chat-messages').append(ich.ChatMessage(chatMessage.toJSON()));
    },

    changeUsers: function () {
        var users = this.chat.chatUsers.toJsonViewModel();
        this.$el.find('.chat-current-users').empty();
        this.$el.find('.chat-current-users').append(ich.ChatUsers({ ChatUsers: users }));
    }
});