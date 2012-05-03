
window.Bowerbird.Views.UserOnlineView = Backbone.View.extend({
    tagName: 'li',

    className: 'user-online-view',

    initialize: function (options) {
        _.extend(this, Backbone.Events);
        this.user = options.user;
    },

    render: function () {
        this.$el.append(ich.UsersOnlineList(this.user.toJSON()));
        return this;
    }
});