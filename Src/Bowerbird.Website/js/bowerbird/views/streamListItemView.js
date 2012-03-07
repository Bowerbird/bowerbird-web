
window.Bowerbird.Views.StreamListItemView = Backbone.View.extend({
    className: 'stream-item',

    observationTemplate: $.template('observationStreamListItemTemplate', $('#observation-stream-list-item-template')),

    postTemplate: $.template('postStreamListItemTemplate', $('#post-stream-list-item-template')),

    initialize: function (options) {
        _.extend(this, Backbone.Events);
        this.streamItem = options.streamItem;
    },

    render: function () {
        var options = {
            formatDate: function () {
                return new Date(parseInt(this.data.Item.ObservedOn.substr(6))).format('d MMM yyyy');
            },
            formatTime: function () {
                return new Date(parseInt(this.data.Item.ObservedOn.substr(6))).format('h:mm');
            }
        };
        switch (this.streamItem.get('Type')) {
            case 'Observation':
                $.tmpl('observationStreamListItemTemplate', this.streamItem.toJSON(), options).appendTo(this.$el);
                this.$el.addClass('observation-stream-item');
                break;
            default:
                break;
        }
        return this;
    }
});