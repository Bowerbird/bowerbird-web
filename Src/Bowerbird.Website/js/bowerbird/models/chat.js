
window.Bowerbird.Models.Chat = Backbone.Model.extend({
    defaults: {
    },
    initialize: function () {
        _.extend(this, Backbone.Events);
        _.bindAll(this);
        this.ChatMessages = new Bowerbird.Collections.ChatMessages();
        this.ChatUsers = new Bowerbird.Collections.ChatUsers();
    }
});