
window.Bowerbird.Views.UserOnlineView = Backbone.View.extend({

    tagName: 'li',

    className: 'user-online-view',
    
    events: {
    },

    //template: $.template('userOnlineTemplate', $('#user-online-template')),

    initialize: function (options) {
        _.extend(this, Backbone.Events);
        this.user = options.user;
    },

    render: function () {
        var usersOnlineHtml = ich.usersonline(this.user.toJSON()).appendTo(this.$el);
        //$.tmpl('userOnlineTemplate', this.user.toJSON()).appendTo(this.$el);
        return this;
    }

});