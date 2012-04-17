
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
        'showProjectCreateFormView',
        'showTeamCreateFormView',
        'showOrganisationCreateFormView',
        'showChatView',
        'removeChatView',
        'showExploreView'
        );
        this.streamView = null;
        this.formView = null;
        this.userView = null;
        this.exploreView = null;
        app.stream.on('newStream', this.showStreamView, this);
        app.on('change:newObservation', this.showObservationCreateFormView, this);
        app.on('change:newProject', this.showProjectCreateFormView, this);
        app.on('change:newTeam', this.showTeamCreateFormView, this);
        app.on('change:newOrganisation', this.showOrganisationCreateFormView, this);
        app.chats.on('remove', this.removeChatView, this);
        app.chats.on('add', this.showChatView, this);
        this.chatViews = [];
    },

    render: function () {
        this.streamView = new Bowerbird.Views.StreamView();
        this.$el.append(this.streamView.render().el);

        this.showOnlineUsers();

        // Closes all popup menus in entire page
        $("body").click(function () {
            $('.sub-menu-button').removeClass('active'); // Make sure to add any new menu button types to the selector
        });

        return this;
    },

    showStreamView: function () {
        if (!app.stream.isSet()) {
            app.stream.setNewStream(null, 'all');
        }
        $(this.streamView.el).show();
        window.scrollTo(0, 0);
    },

    hideFormView: function () {
        if (this.formView) {
            $(this.formView.el).remove();
        }
        this.showStreamView();
    },

    showOnlineUsers: function () {
        log('appView.showOnlineUsers');
        this.userView = new Bowerbird.Views.UserView();
        $('body').append(this.userView.render().el);
    },

    showObservationCreateFormView: function () {
        if (app.has('newObservation')) {
            $(this.streamView.el).hide();
            this.formView = new Bowerbird.Views.ObservationCreateFormView({ observation: app.get('newObservation') });
            this.$el.append(this.formView.render().el);
            this.formView.on('formClosed', this.hideFormView, this);
            this.formView.start();
        }
    },

    showProjectCreateFormView: function () {
        if (app.has('newProject')) {
            $(this.streamView.el).hide();
            this.formView = new Bowerbird.Views.ProjectCreateFormView({ project: app.get('newProject') });
            this.$el.append(this.FormView.render().el);
            this.formView.on('formClosed', this.hideFormView, this);
            this.formView.start();
        }
    },

    showTeamCreateFormView: function () {
        if (app.has('newTeam')) {
            $(this.streamView.el).hide();
            this.formView = new Bowerbird.Views.TeamCreateFormView({ team: app.get('newTeam') });
            this.$el.append(this.formView.render().el);
            this.formView.on('formClosed', this.hideFormView, this);
            this.formView.start();
        }
    },

    showOrganisationCreateFormView: function () {
        if (app.has('newOrganisation')) {
            $(this.streamView.el).hide();
            this.formView = new Bowerbird.Views.OrganisationCreateFormView({ organisation: app.get('newOrganisation') });
            this.$el.append(this.formView.render().el);
            this.formView.on('formClosed', this.HideFormView, this);
            this.formView.start();
        }
    },

    showChatView: function (chat) {
        log('appView.showChatView');
        var chatView = new Bowerbird.Views.ChatView({ chat: chat, id: 'chat-' + chat.Id });
        this.chatViews.push(chatView);
        $('body').append(chatView.render().el);
    },

    removeChatView: function (chat, options) {
        var chatView = _.find(this.chatViews, function (item) {
            return item.Chat === chat;
        });
        chatView.remove();
        this.chatViews = _.without(this.chatViews, chatView);
    },

    newChatRequest: function (e) {
        log('appView.newChatRequest');
    },

    showExploreView: function () {
        if (!app.explore.isSet()) {
            app.explore.setNewExplore(null);
        }
        $(this.streamView.el).show();
        window.scrollTo(0, 0);
    },
});