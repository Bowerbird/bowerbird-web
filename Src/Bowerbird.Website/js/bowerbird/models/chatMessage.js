
window.Bowerbird.Models.ChatMessage = Backbone.Model.extend({

    defaults: {
        User: null,
        Message: null,
        Timestamp: null
    },

    initialize: function (options) {
        _.extend(this, Backbone.Events);

        this.set('User', options.User);
        this.set('Message', options.Message);
        this.set('Timestamp', options.Timestamp);
    }

});