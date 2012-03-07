
window.Bowerbird.ChatManager = Backbone.Model.extend({
    initialize: function (options) {
        _.extend(this, Backbone.Events);
        options.appManager.chats.on('add', this.startChat, this);
        //this.chatHub = $.connection.chatHub;
        //this.chatHub.chatMessageReceived = this.chatMessageReceived;
    },

    startChat: function (chat) {
        // send to signalr

        // when ready, fire trigger
        this.trigger('chatStarted', chat);
    },

    chatMessageReceived: function (data) {
        var chat = app.chats.get(data.groupId);
        chat.chatMessages.add(new Bowerbird.Models.ChatMessage(data));
    }
});