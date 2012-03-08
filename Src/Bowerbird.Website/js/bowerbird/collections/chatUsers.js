
window.Bowerbird.Collections.ChatUsers = Backbone.Collection.extend({
    model: Bowerbird.Models.ChatUser,

    url: '/chatUsers/list',

    initialize: function () {
        _.extend(this, Backbone.Events);
    }
});