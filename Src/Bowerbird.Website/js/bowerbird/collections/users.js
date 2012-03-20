
window.Bowerbird.Collections.Users = Backbone.Collection.extend({
    model: Bowerbird.Models.User,

    url: '/users/list',

    initialize: function () {
        _.extend(this, Backbone.Events);
    }
});