
window.Bowerbird.Views.NotificationsView = Backbone.View.extend({
    
    id: 'notifications',

    initialize: function (options) {
        _.extend(this, Backbone.Events);
        _.bindAll(this, 'addNotification');
        app.notifications.on('add', this.addNotification, this);
        this.notificationItemViews = [];
    },

    render: function () {
        var notificationsHtml = ich.Notifications();
        this.$el.append(notificationsHtml);
        return this;
    },

    addNotification: function (notification) {
        if (notification.get('Action') == 'newobservation') {
            var notificationItemView = new Bowerbird.Views.NotificationItemView({ notification: notification });
            this.notificationItemViews.push(notificationItemView);
            this.$el.find('ul').append(notificationItemView.render().el);
        }
    }
});