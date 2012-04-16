
window.Bowerbird.Views.StreamListItemView = Backbone.View.extend({
    className: 'stream-item',

    initialize: function (options) {
        _.extend(this, Backbone.Events);
        this.StreamItem = options.StreamItem;
    },

    render: function () {
        switch (this.streamItem.get('Type')) {
            case 'observation':
                var streamitemJSON = this.StreamItem.toJSON();
                streamitemJSON['ObservedOnDate'] = new Date(parseInt(this.StreamItem.get('Item').ObservedOn.substr(6))).format('d MMM yyyy');
                streamitemJSON['ObservedOnTime'] = new Date(parseInt(this.StreamItem.get('Item').ObservedOn.substr(6))).format('h:mm');
                streamitemJSON['HighlightMedia'] = streamitemJSON.item.observationMedia[0];
                var streamItemHtml = ich.observationStreamListItem(streamitemJSON);
                this.$el.append(streamItemHtml).addClass('observation-stream-item');
                break;
            default:
                break;
        }
        return this;
    }
});