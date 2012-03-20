
window.Bowerbird.Views.AppView = Backbone.View.extend({
    el: $('article'),

    events: {
        'click #show-online-users': 'showOnlineUsers',
        'click .window-title-bar': 'toggleUserView',
        'click #start-chat': 'showChatView',
        'click .chat-icon': 'startChat'
    },

    UserTemplate: $.template('userTemplate', $('#user-template')),

    initialize: function (options) {
        _.extend(this, Backbone.Events);
        _.bindAll(this, 'showStreamView', 'showObservationCreateFormView', 'startChat', 'showChatView');
        this.streamView = new Bowerbird.Views.StreamView();
        this.$el.append(this.streamView.render().el);
        this.formView = null;
        //this.userView = new Bowerbird.Views.UserView();
        app.stream.on('newStream', this.showStreamView, this);
        app.on('change:newObservation', this.showObservationCreateFormView, this);
        app.chatRouter.on('chatStarted', this.showChatView, this);
        app.chats.on('add', this.showChatView, this);
        app.chats.on('remove', this.removeChatView, this);
        //app.users.on('add', this.addOnlineUser, this);
        //app.users.on('reset', this.addOnlineUsers, this);
        //this.on('showUsers', this.showOnlineUsers, this);
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
        console.log('appView.showChatView');
        var chatView = new Bowerbird.Views.ChatView({ chat: chat, id: 'chat-' + chat.id });
        this.chatViews.push(chatView);
        $('body').append(chatView.render().el);
    },

    showOnlineUsers: function () {
        console.log('appView.showOnlineUsers');
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

    toggleUserView: function () {
        console.log('appView.toggleUserView');

        if ($('#user-window-view-control').hasClass('window-maximize-icon')) //minimize
        {
            $('#user-window-view-control').removeClass('window-maximize-icon')
            $('#user-window-view-control').addClass('window-minimize-icon')
            $('#user-template .window-body').show();
        }
        else //maximise
        {
            $('#user-window-view-control').addClass('window-maximize-icon')
            $('#user-window-view-control').removeClass('window-minimize-icon')
            $('#user-template .window-body').hide();
        }
    },

    startChat: function (e) {
        var groupId = e.target["id"].split('-')[1];

        var chatGroup = '';

        if (groupId.indexOf("projects/") != -1) {
            // this is a project chat
            chatGroup = app.projects.get(groupId);
        } else if (groupId.indexOf("teams/") != -1) {
            // this is a team chat
            chatGroup = app.teams.get(groupId);
        } else {
            // this is a personal chat
            return; // not implemented yet
        }

        //if the chat exists, show it, otherwise make it.
        var chat = app.chats.get('chatId');

        if (chat == 'undefined' || chat == null) {
            chat = new Bowerbird.Models.Chat({ id: groupId, group: chatGroup });
            app.chats.add(chat);
        }

        app.chatRouter.joinChat(chat);

        this.showChatView(chat);
    }

    //    addOnlineUser: function (user) {
    //        console.log('appView.addOnlineUser');
    //        this.userView.addUser(user);
    //        //        var userOnlineView = new Bowerbird.Views.UserOnlineView({ user: user });
    //        //        this.userOnlineViews.push(userOnlineView);
    //    },

    //    addOnlineUsers: function (users) {
    //        console.log('appView.addOnlineUsers');
    //        this.userView.addUsers(users);
    //    }
});