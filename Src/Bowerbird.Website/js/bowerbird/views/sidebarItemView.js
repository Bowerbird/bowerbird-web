
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
        jsonModel['type'] = this.sidebarItemType;
        jsonModel['addObservation'] = this.sidebarItemType === 'Project';
        var sidebarItemHtml = ich.sidebarItemTemplate(jsonModel);
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
        var chat = app.chats.get(this.sidebarItem.id);
        if (chat == null) {
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