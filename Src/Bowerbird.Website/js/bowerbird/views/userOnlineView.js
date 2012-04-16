
window.Bowerbird.Views.UserOnlineView = Backbone.View.extend({

    tagName: 'li',

    className: 'user-online-view',
    
    events: {
    },

    initialize: function (options) {
        _.extend(this, Backbone.Events);
        this.User = options.User;
    },

    render: function () {
        var usersOnlineHtml = ich.usersonline(this.User.toJSON()).appendTo(this.$el);
        return this;
    }

});