
window.Bowerbird.Views.AppView = Backbone.View.extend({
    el: $('article'),

    initialize: function (options) {
        _.extend(this, Backbone.Events);
        _.bindAll(this, 'showStreamView', 'showObservationCreateFormView', 'removeChatView');
        this.streamView = new Bowerbird.Views.StreamView();
        this.$el.append(this.streamView.render().el);
        this.formView = null;
        app.stream.on('newStream', this.showStreamView, this);
        app.on('change:newObservation', this.showObservationCreateFormView, this);
        app.chatManager.on('chatStarted', this.showChatView, this);
        app.chats.on('remove', this.removeChatView, this);
        this.chatViews = [];
    },

    showStreamView: function () {
        if (this.formView) {
            $(this.formView.el).remove();
        }
        $(this.streamView.el).show();
        window.scrollTo(0, 0);
    },

    showObservationCreateFormView: function () {
        if (app.has('newObservation')) {
            $(this.streamView.el).hide();
            this.formView = new Bowerbird.Views.ObservationCreateFormView({ appView: this });
            this.$el.append(this.formView.render().el);
            this.formView.start();
        }
    },

    showChatView: function (chat) {
        console.log('appView.showChatView');
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

    }
});