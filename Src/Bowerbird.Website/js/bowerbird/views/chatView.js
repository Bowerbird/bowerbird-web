
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
        this.Chat = options.Chat;
        this.Chat.ChatUsers.on('add', this.changeUsers, this);
        this.Chat.ChatUsers.on('remove', this.changeUsers, this);
        this.Chat.ChatUsers.on('reset', this.changeUsers, this);
        this.Chat.ChatMessages.on('add', this.addChatMessage, this);
    },

    render: function () {
        var chatTemplate = ich.chatwindow({ Title: this.Chat.get('Title'), Messages: this.Chat.ChatMessages.toJSON(), ChatUsers: this.Chat.ChatUsers.toJSON() });
        this.$el.append(chatTemplate);
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
        var messageTemplate = ich.chatmessage(chatMessage.toJSON());
        this.$el.find('.chat-messages').append(messageTemplate);
    },

    changeUsers: function () {
        var users = this.Chat.ChatUsers.toJsonViewModel();
        var chatUsers = ich.chatusers({ ChatUsers: users });
        this.$el.find('.chat-current-users').empty();
        this.$el.find('.chat-current-users').append(chatUsers);
    }
});