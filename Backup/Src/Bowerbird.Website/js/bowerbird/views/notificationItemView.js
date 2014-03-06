/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// NotificationItemView
// --------------------

define(['jquery', 'underscore', 'backbone', 'app', 'moment', 'timeago'], 
    function ($, _, Backbone, app, moment) {

//    var parseISO8601 = function (str) {
//        // we assume str is a UTC date ending in 'Z'

//        var parts = str.split('T'),
//        dateParts = parts[0].split('-'),
//        timeParts = parts[1].split('Z'),
//        timeSubParts = timeParts[0].split(':'),
//        timeSecParts = timeSubParts[2].split('.'),
//        timeHours = Number(timeSubParts[0]),
//        _date = new Date;

//        _date.setUTCFullYear(Number(dateParts[0]));
//        _date.setUTCMonth(Number(dateParts[1]) - 1);
//        _date.setUTCDate(Number(dateParts[2]));
//        _date.setUTCHours(Number(timeHours));
//        _date.setUTCMinutes(Number(timeSubParts[1]));
//        _date.setUTCSeconds(Number(timeSecParts[0]));
//        if (timeSecParts[1]) _date.setUTCMilliseconds(Number(timeSecParts[1]));

//        // by using setUTC methods the date has already been converted to local time(?)
//        return _date;
//    };

    var NotificationItemView = Backbone.Marionette.ItemView.extend({
        tagName: 'li',

        className: 'notification-item',

        template: 'NotificationItem',

        serializeData: function () {
            return {
                Model: {
                    Activity: this.model.toJSON(),
                    CreatedDateTimeDescription: moment(this.model.get('CreatedDateTime')).format('D MMM YYYY h:mma')
                }
            };
        },

        onRender: function () {
            this.$el.find('.time-description').timeago();
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

    return NotificationItemView;

});