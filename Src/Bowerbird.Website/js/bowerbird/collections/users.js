
window.Bowerbird.Collections.Users = Backbone.Collection.extend({
    model: Bowerbird.Models.User,

    url: '/users/list',

    initialize: function () {
        _.extend(this, Backbone.Events);
    },

    updateUserStatus: function (newUser) {
        if (!this.has(newUser.id)) {
            if (newUser.status == 2 || newUser.status == 3 || newUser.status == 'undefined') return;
            var user = new Bowerbird.Models.User(newUser);
            app.onlineUsers.add(user);
            log('app.userStatusUpdate: ' + newUser.name + ' logged in');
        } else {
            var user = this.get(newUser.id);
            if (newUser.status == 2 || newUser.status == 3) {
                app.onlineUsers.remove(user);
                log('app.userStatusUpdate: ' + newUser.name + ' logged out');
            } else {
                user.set('status', newUser.status);
                log('app.userStatusUpdate: ' + newUser.name + ' udpated their status');
            }
        }
    }
});