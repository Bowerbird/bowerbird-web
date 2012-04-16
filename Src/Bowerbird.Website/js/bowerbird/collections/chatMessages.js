
window.Bowerbird.Collections.ChatMessages = Backbone.Collection.extend({
    
    model: Bowerbird.Models.ChatMessage,

    url: '/chatMessages/list',

    initialize: function () {
        _.extend(this, Backbone.Events);
    }
    
});