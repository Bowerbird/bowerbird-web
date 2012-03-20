
window.Bowerbird.Views.ChatViewUser = Backbone.View.extend({

    tagName: 'li',

    className: 'chat-view-user',

    events: {
    },

    template: $.template('chatUserTemplate', $('#chat-user-template')),

    chatUserStatusTemplate: $.template('chatUserStatusTemplate', $('#chat-user-status')),


    initialize: function (options) {
        _.extend(this, Backbone.Events);
        this.chatUser = options.user;
        this.chatUser.on('statusChange', this.changeUserStatus, this);
    },

    render: function () {
        //$.tmpl('chatUserTemplate', this.chatUser.toJSON()).appendTo(this.$el);
        $.tmpl('chatViewUserTemplate', chatUser.toJSON()).appendTo(this.$el.find('.chat-current-users'));
        $.tmpl('chatUserStatusTemplate', this.chatUser.toJSON()).appendTo(this.$el.find('.chat-user-status'));
        return this;
    },

    changeUserStatus: function (chatUser) {
        $.tmpl('chatUserStatusTemplate', this.chatUser.toJSON()).appendTo(this.$el.find('.chat-user-status'));
    }
});