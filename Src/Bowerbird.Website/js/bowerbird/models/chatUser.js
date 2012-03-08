
window.Bowerbird.Models.ChatUser = Backbone.Model.extend({
    defaults: {
        chat: null
    },
    initialize: function (options) {
        _.extend(this, Backbone.Events);
        //_.bindAll(this);
        this.set('chat', options.chat);
    }
});