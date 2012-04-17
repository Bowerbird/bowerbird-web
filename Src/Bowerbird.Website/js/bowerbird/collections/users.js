
window.Bowerbird.Collections.Users = Backbone.Collection.extend({
    model: Bowerbird.Models.User,

    url: '/users/list',

    initialize: function () {
        _.extend(this, Backbone.Events);
        _.bindAll(this, 'updateUserStatus');
    },

    updateUserStatus: function (newUser) {
        if (!this.contains(newUser.Id)) {
            if (newUser.Status == 2 || newUser.Status == 3 || newUser.Status == 'undefined') return;
            var user = new Bowerbird.Models.User(newUser);
            app.onlineUsers.add(user);
        } else {
            var user = this.get(newUser.Id);
            if (newUser.Status == 2 || newUser.Status == 3) {
                app.onlineUsers.remove(user);
            } else {
                user.set('Status', newUser.Status);
            }
        }
    }
});