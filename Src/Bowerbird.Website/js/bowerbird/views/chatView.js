
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
        this.chat.chatMessages.on('add', this.addChatMessage, this);
        this.chat.chatUsers.on('add', this.changeUsers, this);
        this.chat.chatUsers.on('remove', this.changeUsers, this);
        this.chat.chatUsers.on('reset', this.changeUsers, this);
    },

    render: function () {
        var chatTemplate = ich.chatwindow({ title: this.chat.get('title'), messages: this.chat.chatMessages.toJSON(), chatUsers: this.chat.chatUsers.toJSON() });
        this.$el.append(chatTemplate);
        return this;
    },

    sendMessage: function () {
        log('chatView.sendMessage');
        app.chatRouter.sendMessage(this.$el.find('.new-chat-message').val(), this.chat);
        this.$el.find('.new-chat-message').val('');
    },

    closeWindow: function () {
        app.chats.remove(this.chat);
        this.$el.remove();
    },

    addChatMessage: function (chatMessage) {
        log('chatView.addChatMessage');
        var messageTemplate = ich.chatmessage(chatMessage.toJSON());
        this.$el.find('.chat-messages').append(messageTemplate);
    },

    changeUsers: function () {
        var users = this.chat.chatUsers.toJsonViewModel();
        var chatUsers = ich.chatusers({ chatUsers: users });
        this.$el.find('.chat-current-users').empty();
        this.$el.find('.chat-current-users').append(chatUsers);
    }
});