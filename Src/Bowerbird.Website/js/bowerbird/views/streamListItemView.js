
window.Bowerbird.Views.StreamListItemView = Backbone.View.extend({
    className: 'stream-item',

    //observationTemplate: $.template('observationStreamListItemTemplate', $('#observation-stream-list-item-template')),

    //postTemplate: $.template('postStreamListItemTemplate', $('#post-stream-list-item-template')),

    initialize: function (options) {
        _.extend(this, Backbone.Events);
        this.streamItem = options.streamItem;
    },

    render: function () {
        switch (this.streamItem.get('type')) {
            case 'observation':
                var streamitemJSON = this.streamItem.toJSON();
                streamitemJSON['observedOnDate'] = new Date(parseInt(this.streamItem.get('item').observedOn.substr(6))).format('d MMM yyyy');
                streamitemJSON['observedOnTime'] = new Date(parseInt(this.streamItem.get('item').observedOn.substr(6))).format('h:mm');
                streamitemJSON['highlightMedia'] = streamitemJSON.item.observationMedia[0];
                var streamItemHtml = ich.observationStreamListItemTemplate(streamitemJSON);
                this.$el.append(streamItemHtml).addClass('observation-stream-item');
                break;
            default:
                break;
        }
        return this;
    }
});