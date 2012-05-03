
window.Bowerbird.Models.UserChat = Bowerbird.Models.Chat.extend({
    initialize: function (options) {
        this.constructor.__super__.initialize.apply(this, [options])
        _.extend(this, Backbone.Events);
        _.bindAll(this);
        this.chatUsers.add(options.user);
        var chatMessage = new Bowerbird.Models.ChatMessage(options);
        this.chatMessages.add(chatMessage);
    }
});