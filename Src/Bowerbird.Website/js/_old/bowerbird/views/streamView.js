
window.Bowerbird.Views.StreamView = Backbone.View.extend({
    id: 'stream',

    initialize: function (options) {
        _.extend(this, Backbone.Events);
        this.sidebarView = new Bowerbird.Views.SidebarView();
        this.streamListView = new Bowerbird.Views.StreamListView();
    },

    render: function () {
        this.$el.append(this.sidebarView.render().el);
        this.$el.append(this.streamListView.render().el);
        return this;
    }
});