
window.Bowerbird.Views.NotificationListView = Backbone.View.extend({
    id: 'notifications',

    initialize: function (options) {
        _.extend(this, Backbone.Events);
    },

    render: function () {
        return this;
    }
});