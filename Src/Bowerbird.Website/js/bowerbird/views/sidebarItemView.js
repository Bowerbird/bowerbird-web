
window.Bowerbird.Views.SidebarItemView = Backbone.View.extend({
    tagName: 'li',

    className: 'menu-group-item',

    events: {
        'click .chat-menu-item': 'startChat',
        'click .sub-menu-button': 'showMenu',
        'click .sub-menu-button li': 'selectMenuItem'
    },

    initialize: function (options) {
        _.extend(this, Backbone.Events);
        _.bindAll(this, 'startChat');
        this.sidebarItemType = options.type;
        this.sidebarItem = options.sidebarItem;
        this.streamItemNotificationCount = 0;
        app.notifications.on('streamItemNotificationAdded', this.streamItemNotificationAdded, this);
    },

    render: function () {
        var jsonModel = this.sidebarItem.toJSONViewModel();
        jsonModel['Type'] = this.sidebarItemType;
        jsonModel['AddObservation'] = this.sidebarItemType === 'Project';
        this.$el.append(ich.sidebarItem(jsonModel));
        return this;
    },

    showMenu: function (e) {
        $('.sub-menu-button').removeClass('active');
        $(e.currentTarget).addClass('active');
        e.stopPropagation();
    },

    selectMenuItem: function (e) {
        $('.sub-menu-button').removeClass('active');
        e.stopPropagation();
    },

    startChat: function (e) {
        var chat = app.chats.get(this.sidebarItem.Id);
        if (chat == null) {
            chat = new Bowerbird.Models.GroupChat({ Id: this.sidebarItem.Id, Group: this.sidebarItem });
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