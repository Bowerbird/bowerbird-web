/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// StreamItemView
// --------------

// Shows an individual stream item
define(['jquery', 'underscore', 'backbone', 'app'], function ($, _, Backbone, app) {

    var StreamItemView = Backbone.Marionette.ItemView.extend({
        tagName: 'li',

        className: 'stream-item',

        template: 'StreamItem',

        serializeData: function () {
            return {
                Model: this.model.toJSON()
            };
        }
        //        render: function () {
        //            switch (this.StreamItem.get('Type')) {
        //                case 'observation':
        //                    var streamitemJSON = this.streamItem.toJSON();
        //                    streamitemJSON['ObservedOnDate'] = new Date(parseInt(this.streamItem.get('Item').ObservedOn.substr(6))).format('d MMM yyyy');
        //                    streamitemJSON['ObservedOnTime'] = new Date(parseInt(this.streamItem.get('Item').ObservedOn.substr(6))).format('h:mm');
        //                    streamitemJSON['HighlightMedia'] = streamitemJSON.item.observationMedia[0];
        //                    this.$el.append(ich.ObservationStreamListItem(streamitemJSON)).addClass('observation-stream-item');
        //                    break;
        //                default:
        //                    break;
        //            }
        //            return this;
        //        }
    });

    return StreamItemView;

});