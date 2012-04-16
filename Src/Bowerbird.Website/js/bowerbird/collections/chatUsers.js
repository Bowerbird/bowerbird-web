
window.Bowerbird.Collections.ChatUsers = Backbone.Collection.extend({
    
    model: Bowerbird.Models.ChatUser,

    url: '/chatUsers/list',

    initialize: function () {
        _.extend(this, Backbone.Events);
    }
    ,

    toJsonViewModel: function () {
        var users = this.pluck('User');
        var jsonUsers = _.map(users, function (user) {
            return { 
                "Id": User.Id,
                "Name": User.get('Name'), 
                "Avatar": User.get('Avatar')
            };
        });

        return jsonUsers;
    }
});