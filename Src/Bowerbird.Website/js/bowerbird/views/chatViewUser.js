
window.Bowerbird.Views.ChatViewUser = Backbone.View.extend({

    tagName: 'li',

    className: 'chat-view-user',

    events: {
       
    },

    template: $.template('chatUserTemplate', $('#chat-user-template')),

    initialize: function (options) {
        _.extend(this, Backbone.Events);
        this.chatUser = options.user;
    },

    render: function () {
        $.tmpl('chatUserTemplate', this.chatUser.toJSON()).appendTo(this.$el);
        return this;
    }
});