
window.Bowerbird.Views.SidebarItemView = Backbone.View.extend({
    tagName: 'li',

    template: $.template('sidebarItemTemplate', $('#sidebar-item-template')),

    events: {
        'click .chat-icon': 'startChat'
    },

    initialize: function (options) {
        _.extend(this, Backbone.Events);
        this.sidebarItem = options.sidebarItem;
    },

    render: function () {
        $.tmpl("sidebarItemTemplate", this.sidebarItem.toJSON()).appendTo(this.$el);
        return this;
    },

    startChat: function (e) {
        // call can come from a user's chat-icon or a team or project's chat-icon
        var id = e.target["id"].split('-')[1];
        if (id.indexOf("projects/") != -1) {// project chat
            var chatGroup = app.projects.get(id);
            // grab chat if exists or create and add
            var chat = app.chats.get(id);
            if (_.isNull(chat) || _.isUndefined(chat)) {
                chat = new Bowerbird.Models.GroupChat({ id: id, group: chatGroup });
                app.chats.add(chat);
            }
        }
        else if (id.indexOf("teams/") != -1) {// team chat
            var chatGroup = app.teams.get(id);
            // grab chat if exists or create and add
            var chat = app.chats.get(id);
            if (_.isNull(chat) || _.isUndefined(chat)) {
                chat = new Bowerbird.Models.GroupChat({ id: id, group: chatGroup });
                app.chats.add(chat);
            }
        }
        app.chatRouter.joinChat(chat);
    },
});