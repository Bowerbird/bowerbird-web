
window.Bowerbird.Models.ChatMessage = Backbone.Model.extend({

    defaults: {
        user: null,
        message: null,
        timestamp: null
    },

    initialize: function (options) {
        _.extend(this, Backbone.Events);

        this.set('user', options.user);
        this.set('message', options.message);
        this.set('timestamp', options.timestamp);
    }

});