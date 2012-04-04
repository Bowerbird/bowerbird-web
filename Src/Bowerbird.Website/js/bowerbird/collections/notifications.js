
window.Bowerbird.Collections.Notifications = Backbone.Collection.extend({
    model: Bowerbird.Models.Notification,

    url: '/notifications/list',

    initialize: function () {
        _.extend(this, Backbone.Events);
        //this.on('add', this.addNotification, this);
    }
});