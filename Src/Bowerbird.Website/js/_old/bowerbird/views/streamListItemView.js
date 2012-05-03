
window.Bowerbird.Views.StreamListItemView = Backbone.View.extend({
    className: 'stream-item',

    initialize: function (options) {
        _.extend(this, Backbone.Events);
        this.streamItem = options.streamItem;
    },

    render: function () {
        switch (this.StreamItem.get('Type')) {
            case 'observation':
                var streamitemJSON = this.streamItem.toJSON();
                streamitemJSON['ObservedOnDate'] = new Date(parseInt(this.streamItem.get('Item').ObservedOn.substr(6))).format('d MMM yyyy');
                streamitemJSON['ObservedOnTime'] = new Date(parseInt(this.streamItem.get('Item').ObservedOn.substr(6))).format('h:mm');
                streamitemJSON['HighlightMedia'] = streamitemJSON.item.observationMedia[0];
                this.$el.append(ich.ObservationStreamListItem(streamitemJSON)).addClass('observation-stream-item');
                break;
            default:
                break;
        }
        return this;
    }
});