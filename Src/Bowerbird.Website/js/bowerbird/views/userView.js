
window.Bowerbird.Views.UserView = Backbone.View.extend({
    className: 'user-window',

    events: {
        "click .window-title-bar": "toggleWindowView"
    },

    initialize: function () {
        _.extend(this, Backbone.Events);
        _.bindAll(this, 'change');
        this.fullView = false;
        app.users.on('add', this.change, this);
        app.users.on('remove', this.change, this);
    },

    render: function () {
        var usersTemplate = ich.usersonline({ count: app.users.length, users: app.users.toJSON() });
        this.$el.append(usersTemplate);
        return this;
    },

    toggleWindowView: function () {
        if (this.fullView) {
            this.fullView = false;
            this.$el.animate({ bottom: "-180px", duration: "slow", easing: "easein" });
        } else {
            this.fullView = true;
            this.$el.animate({ bottom: "-10px", duration: "slow", easing: "easein" });
        }
    },

    change: function () {
        var usersTemplate = ich.usersonline({ count: app.users.length, users: app.users.toJSON() });
        this.$el.empty();
        this.$el.append(usersTemplate);
    } 
});