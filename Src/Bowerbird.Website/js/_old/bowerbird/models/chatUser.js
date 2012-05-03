
window.Bowerbird.Models.ChatUser = Backbone.Model.extend({
    defaults: {
        Chat: null,
        User: null,
        Status: null
    },

    initialize: function (options) {
        _.extend(this, Backbone.Events);
    }
});