
window.Bowerbird.Views.SidebarItemView = Backbone.View.extend({
    tagName: 'li',

    template: $.template('sidebarItemTemplate', $('#sidebar-item-template')),

    initialize: function (options) {
        _.extend(this, Backbone.Events);
        this.sidebarItem = options.sidebarItem;
    },

    render: function () {
        $.tmpl("sidebarItemTemplate", this.sidebarItem.toJSON()).appendTo(this.$el);
        return this;
    }
});