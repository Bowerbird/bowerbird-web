
window.Bowerbird.Views.UserView = Backbone.View.extend({
    className: 'user-window',

    events: {
        'click .window-title-bar': 'toggleWindowView',
        'click .user-chat-icon': 'startChat'
    },

    initialize: function () {
        _.extend(this, Backbone.Events);
        _.bindAll(this,
        'toggleWindowView',
        'render',
        'startChat',
        'generateGuid');
        this.fullView = false;
        app.onlineUsers.on('add', this.render, this);
        app.onlineUsers.on('remove', this.render, this);
        app.onlineUsers.on('reset', this.render, this);
    },

    render: function () {
        this.$el.empty();
        this.$el.append(ich.UsersOnlineList({ Count: app.onlineUsers.length, Users: app.onlineUsers.toJSON() }));
        return this;
    },

    toggleWindowView: function () {
        if (this.fullView) {
            this.fullView = false;
            this.$el.animate({ bottom: "-180px", duration: "slow", easing: "easein" });
        } else {
            this.fullView = true;
            this.$el.animate({ bottom: "-10px", duration: "slow", easing: "easein" });
        }
    },

    startChat: function (e) {
        // call can come from a user's chat-icon
        var id = e.target["Id"].split('-')[1];
        var user = app.onlineUsers.get(Id);
        var chatId = this.generateGuid();
        var chat = new Bowerbird.Models.UserChat({ Id: chatId, User: user });
        app.chats.add(chat);
        app.chatRouter.joinChat(chat);
    },

    generateGuid: function () {
        var S4 = function () {
            return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
        };
        return (S4() + S4() + "-" + S4() + "-" + S4() + "-" + S4() + "-" + S4() + S4() + S4());
    }

});