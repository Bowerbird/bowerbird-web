
window.Bowerbird.Collections.ChatUsers = Backbone.Collection.extend({
    model: Bowerbird.Models.ChatUser,

    url: '/chatUsers/list',

    initialize: function () {
        _.extend(this, Backbone.Events);
    }
    ,

    toJsonViewModel: function () {
        var users = this.pluck('user');
        var jsonUsers = _.map(users, function (user) {
            return { 
                "id": user.id,
                "name": user.get('name'), 
                "avatar": user.get('avatar')
            };
        });

        return jsonUsers;
    }
});