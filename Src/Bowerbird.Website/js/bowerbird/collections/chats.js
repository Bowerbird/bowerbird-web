
window.Bowerbird.Collections.Chats = Backbone.Collection.extend({
    
    model: Bowerbird.Models.Chat,

    url: '/chats/list',

    initialize: function () {
        _.extend(this, Backbone.Events);
    }
});