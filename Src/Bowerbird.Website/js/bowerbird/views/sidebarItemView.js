
window.Bowerbird.Views.SidebarItemView = Backbone.View.extend({
    tagName: 'li',

    template: $.template('sidebarItemTemplate', $('#sidebar-item-template')),

    events: {
        'click .chat-icon': 'startChat'
    },

    initialize: function (options) {
        _.extend(this, Backbone.Events);
        _.bindAll(this, 'startChat');
        this.sidebarItem = options.sidebarItem;
        this.streamItemNotificationCount = 0;
        app.notifications.on('streamItemNotificationAdded', this.streamItemNotificationAdded, this);
    },

    render: function () {
        $.tmpl("sidebarItemTemplate", this.sidebarItem.toJSONViewModel()).appendTo(this.$el);
        log($.tmpl("sidebarItemTemplate", this.sidebarItem.toJSONViewModel()));
        return this;
    },

    startChat: function (e) {
        var chat = app.chats.get(this.sidebarItem.id);
        if (_.isNull(chat) || _.isUndefined(chat)) {
            chat = new Bowerbird.Models.GroupChat({ id: this.sidebarItem.id, group: this.sidebarItem });
            app.chats.add(chat);
        }
        app.chatRouter.joinChat(chat);
    },

    streamItemNotificationAdded: function (streamItemNotification) {
//        if (this.notification.model.id === this.sidebarItem.id) {
//            this.streamItemNotificationCount++;
//            // increment count in view
//            this.$el.find('').text(this.streamItemNotificationCount);
//        }
    }
});