
window.Bowerbird.Models.ChatUser = Backbone.Model.extend({
    defaults: {
        chat: null,
        user: null,
        status: null
    },
    initialize: function (options) {
        _.extend(this, Backbone.Events);
        this.set('chat', options.chat);
        this.set('user', options.user);
        this.set('status', options.status);
    }
});