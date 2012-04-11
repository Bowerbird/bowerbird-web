
window.Bowerbird.Views.NotificationItemView = Backbone.View.extend({
    tagName: 'li',

    initialize: function (options) {
        _.extend(this, Backbone.Events);
        _.bindAll(this, 'render');
        this.notification = options.notification;
    },

    render: function () {
        var notificationsItemHtml = ich.notificationsItem(this.notification.toJSON());
        this.$el.append(notificationsItemHtml);
        return this;
    }
});