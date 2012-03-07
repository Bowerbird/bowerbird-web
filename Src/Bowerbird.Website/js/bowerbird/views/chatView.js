
window.Bowerbird.Views.ChatView = Backbone.View.extend({
    className: 'chat-window',

    events: {
        "click #chat-send-message-button": "sendMessage"
    },

    template: $.template('chatTemplate', $('#chat-template')),

    chatMessageTemplate: $.template('chatMessageTemplate', $('#chat-message-template')),

    initialize: function (options) {
        _.extend(this, Backbone.Events);
        this.chat = options.chat;
        this.chat.chatMessages.on('add', this.addChatMessage, this);
    },

    render: function () {
        $.tmpl('chatTemplate', this.chat.toJSON()).appendTo(this.$el);
        return this;
    },

    sendMessage: function () {
        console.log('send message!');
    },

    addChatMessage: function (chatMessage) {
        $.tmpl('chatMessageTemplate', chatMessage.toJSON()).appendTo(this.$el.find('.chat-messages'));
    }
});