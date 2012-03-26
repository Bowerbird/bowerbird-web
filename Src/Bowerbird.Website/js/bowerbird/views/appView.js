﻿
window.Bowerbird.Views.AppView = Backbone.View.extend({
    el: $('article'),

    events: {
        'chatRequest': 'newChatRequest'
    },

    initialize: function (options) {
        _.extend(this, Backbone.Events);
        _.bindAll(this,
        'render',
        'showStreamView',
        'showOnlineUsers',
        'showObservationCreateFormView',
        'showChatView',
        'removeChatView'
        );
        this.streamView = null;
        this.formView = null;
        this.userView = null;
        app.stream.on('newStream', this.showStreamView, this);
        app.on('change:newObservation', this.showObservationCreateFormView, this);
        app.chats.on('remove', this.removeChatView, this);
        app.chats.on('add', this.showChatView, this);
        this.chatViews = [];
    },

    render: function () {
        this.showStreamView();
        this.showOnlineUsers();
        return this;
    },

    showStreamView: function () {
        if (this.streamView == null) {
            this.streamView = new Bowerbird.Views.StreamView();
            this.$el.append(this.streamView.render().el);
        }
        if (this.formView) {
            $(this.formView.el).remove();
        }
        $(this.streamView.el).show();
        window.scrollTo(0, 0);
    },

    showOnlineUsers: function () {
        log('appView.showOnlineUsers');
        this.userView = new Bowerbird.Views.UserView();
        $('body').append(this.userView.render().el);
    },

    showObservationCreateFormView: function () {
        if (app.has('newObservation')) {
            $(this.streamView.el).hide();
            this.formView = new Bowerbird.Views.ObservationCreateFormView({ appView: this, observation: app.get('newObservation') });
            this.$el.append(this.formView.render().el);
            this.formView.start();
        }
    },

    showChatView: function (chat) {
        log('appView.showChatView');
        var chatView = new Bowerbird.Views.ChatView({ chat: chat, id: 'chat-' + chat.id });
        this.chatViews.push(chatView);
        $('body').append(chatView.render().el);
    },

    removeChatView: function (chat, options) {
        var chatView = _.find(this.chatViews, function (item) {
            return item.chat === chat;
        });
        chatView.remove();
        this.chatViews = _.without(this.chatViews, chatView);
    },

    newChatRequest: function (e) {
        log('appView.newChatRequest');
    }
});