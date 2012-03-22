
window.Bowerbird.Views.AppView = Backbone.View.extend({
    el: $('article'),

    events: {
        'click .chat-icon': 'startChat'
    },

    initialize: function (options) {
        _.extend(this, Backbone.Events);
        _.bindAll(this,
        'showStreamView',
        'showObservationCreateFormView',
        'showChatView',
        'showOnlineUsers',
        'removeChatView',
        'startChat');
        this.streamView = new Bowerbird.Views.StreamView();
        this.$el.append(this.streamView.render().el);
        this.formView = null;
        app.stream.on('newStream', this.showStreamView, this);
        app.on('change:newObservation', this.showObservationCreateFormView, this);
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

    showOnlineUsers: function () {
        log('appView.showOnlineUsers');
        var userView = new Bowerbird.Views.UserView();
        $('body').append(userView.render().el);
    },

    removeChatView: function (chat, options) {
        var chatView = _.find(this.chatViews, function (item) {
            return item.chat === chat;
        });
        chatView.remove();
        this.chatViews = _.without(this.chatViews, chatView);
    },

    startChat: function (e) {

        var groupId = e.target["id"].split('-')[1];
        var chatGroup = '';

        if (groupId.indexOf("projects/") != -1) {
            chatGroup = app.projects.get(groupId);
        }
        else if (groupId.indexOf("teams/") != -1) {
            chatGroup = app.teams.get(groupId);
        }
        else {
            return; // not implemented yet but will generate a Guid for a personal chat's id
        }

        var chat = app.chats.get(groupId);

        if (_.isNull(chat) || _.isUndefined(chat)) {
            chat = new Bowerbird.Models.Chat({ id: groupId, group: chatGroup });
            app.chats.add(chat);
        }

        app.chatRouter.joinChat(chat);

        this.showChatView(chat);
    }
});