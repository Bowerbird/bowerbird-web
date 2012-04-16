
window.Bowerbird.Views.NotificationItemView = Backbone.View.extend({
    tagName: 'li',

    initialize: function (options) {
        _.extend(this, Backbone.Events);
        _.bindAll(this, 'render');
        this.Notification = options.Notification;
    },

    render: function () {
        var notificationsItemHtml = ich.notificationsItem(this.Notification.toJSON());
        this.$el.append(notificationsItemHtml);
        return this;
    }
});