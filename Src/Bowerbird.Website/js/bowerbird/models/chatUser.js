
window.Bowerbird.Models.ChatUser = Backbone.Model.extend({
    defaults: {
        Chat: null,
        User: null,
        Status: null
    },
    initialize: function (options) {
        _.extend(this, Backbone.Events);
        this.set('Chat', options.Chat);
        this.set('User', options.User);
        this.set('Status', options.Status);
    }
});