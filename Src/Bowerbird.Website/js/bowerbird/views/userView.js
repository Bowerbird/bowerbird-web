
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
        'generateGuid'
        );
        this.fullView = false;
        app.users.on('add', this.render, this);
        app.users.on('remove', this.render, this);
        app.users.on('reset', this.render, this);
    },

    render: function () {
        var usersTemplate = ich.usersonline({ count: app.users.length, users: app.users.toJSON() });
        this.$el.empty();
        this.$el.append(usersTemplate);
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

//    change: function () {
//        var usersTemplate = ich.usersonline({ count: app.users.length, users: app.users.toJSON() });
//        this.$el.empty();
//        this.$el.append(usersTemplate);
//    },

    startChat: function (e) {
        // call can come from a user's chat-icon
        var id = e.target["id"].split('-')[1];
        var user = app.users.get(id);
        var chatId = this.generateGuid();
        var chat = new Bowerbird.Models.UserChat({ id: chatId, user: user });
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