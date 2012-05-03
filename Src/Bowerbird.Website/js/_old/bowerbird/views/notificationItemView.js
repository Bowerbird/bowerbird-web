
window.Bowerbird.Views.NotificationItemView = Backbone.View.extend({
    tagName: 'li',

    initialize: function (options) {
        _.extend(this, Backbone.Events);
        _.bindAll(this, 'render');
        this.notification = options.notification;
    },

    render: function () {
        this.$el.append(ich.NotificationsItem(this.notification.toJSON()));
        return this;
    }
});