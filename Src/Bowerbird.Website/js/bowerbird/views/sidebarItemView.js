
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
        this.SidebarItemType = options.Type;
        this.SidebarItem = options.SidebarItem;
        this.StreamItemNotificationCount = 0;
        app.notifications.on('streamItemNotificationAdded', this.streamItemNotificationAdded, this);
    },

    render: function () {
        var jsonModel = this.SidebarItem.toJSONViewModel();
        jsonModel['Type'] = this.SidebarItemType;
        jsonModel['AddObservation'] = this.SidebarItemType === 'Project';
        var sidebarItemHtml = ich.sidebarItem(jsonModel);
        this.$el.append(sidebarItemHtml);

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
        var chat = app.chats.get(this.SidebarItem.Id);
        if (chat == null) {
            chat = new Bowerbird.Models.GroupChat({ Id: this.SidebarItem.Id, Group: this.SidebarItem });
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