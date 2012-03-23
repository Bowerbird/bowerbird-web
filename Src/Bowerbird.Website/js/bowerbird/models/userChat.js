
window.Bowerbird.Models.UserChat = Bowerbird.Models.Chat.extend({
    defaults: {
    },
    initialize: function (options) {
        this.constructor.__super__.initialize.apply(this, [options])
        _.extend(this, Backbone.Events);
        _.bindAll(this);
        this.chatUsers.add(options.user);
        this.chatMessages.add(options.message);
    }
});