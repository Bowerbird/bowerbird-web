
window.Bowerbird.Collections.ChatUsers = Backbone.Collection.extend({
    model: Bowerbird.Models.ChatUser,

    url: '/chatUsers/list',

    initialize: function () {
        _.extend(this, Backbone.Events);
    }
    ,

    toJsonViewModel: function () {
        // extracts each of the user objects from 
        // the collections chatuser objects and serializes

        //        var jsonViewModel = {
        //            "users": this.pluck('user')
        //        };

        //        return jsonViewModel;
        //return this.pluck('user');
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