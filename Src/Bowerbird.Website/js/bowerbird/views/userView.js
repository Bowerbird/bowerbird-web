
window.Bowerbird.Views.UserView = Backbone.View.extend({
    className: 'user-window',

    events: {
        //        "click .chat-send-message-button": "sendMessage",
                "click .window-close": "closeWindow"
    },

    template: $.template('userTemplate', $('#user-template')),

    userOnlineTemplate: $.template('userOnlineTemplate', $('#user-online-template')),

    initialize: function () {
        _.extend(this, Backbone.Events);
        _.bindAll(this, 'addUser', 'addUsers');
        this.userOnlineViews = [];
        app.users.on('add', this.addUser, this);
        app.users.on('remove', this.removeUser, this);
    },

    render: function () {
        $.tmpl('userTemplate', { count: app.users.length }).appendTo(this.$el);
        this.addUsers(app.users);
        return this;
    },

    closeWindow: function () {
        this.$el.remove();
    },

    addUser: function (user) {
        console.log('userView.addUser');
        var userOnlineView = new Bowerbird.Views.UserOnlineView({ user: user });
        this.userOnlineViews.push(userOnlineView);
        $.tmpl('userOnlineTemplate', user.toJSON()).appendTo(this.$el.find('.online-users'));
        this.$el.find('#users-online').val(app.users.length);
    },

    removeUser: function (user) {
        console.log('userView.removeUser');
        // remove the userOnlineView from the collection...
        //var userOnlineView = new Bowerbird.Views.UserOnlineView({ user: user });
        //this.userOnlineViews.push(userOnlineView);
        //$.tmpl('userOnlineTemplate', user.toJSON()).appendTo(this.$el.find('.online-users'));
        //this.$el.find('#users-online').val(app.users.length);
    },

    addUsers: function (users) {
        console.log('userView.addUsers');
        var self = this;
        users.each(function (user) {
            self.addUser(user);
        });
    }
});