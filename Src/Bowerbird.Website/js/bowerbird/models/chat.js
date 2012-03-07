
window.Bowerbird.Models.Chat = Backbone.Model.extend({
    defaults: {
        group: null
    },

    initialize: function (options) {
        _.extend(this, Backbone.Events);
        _.bindAll(this);
        this.chatMessages = new Bowerbird.Collections.ChatMessages();
        this.set('group', options.group);
    }
});