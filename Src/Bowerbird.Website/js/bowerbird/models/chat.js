
window.Bowerbird.Models.Chat = Backbone.Model.extend({
    defaults: {
    },
    initialize: function () {
        _.extend(this, Backbone.Events);
        _.bindAll(this);
        this.chatMessages = new Bowerbird.Collections.ChatMessages();
        this.chatUsers = new Bowerbird.Collections.ChatUsers();
    }
});