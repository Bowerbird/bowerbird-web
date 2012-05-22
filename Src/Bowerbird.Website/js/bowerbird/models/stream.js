///// <reference path="../../libs/log.js" />
///// <reference path="../../libs/require/require.js" />
///// <reference path="../../libs/jquery/jquery-1.7.2.js" />
///// <reference path="../../libs/underscore/underscore.js" />
///// <reference path="../../libs/backbone/backbone.js" />
///// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

//// Stream
//// ------

//define(['jquery', 'underscore', 'backbone', 'app'], function ($, _, Backbone, app) {

//    var Stream = Backbone.Model.extend({
//        defaults: {
//            Group: null,
//            User: null
//        },

//        initialize: function (options) {
//            _.extend(this, Backbone.Events);
//            _.bindAll(this);
//            //this.streamItems = new Bowerbird.Collections.StreamItems();
//        }

////        isSet: function () {
////            return this.get('Filter') != null && this.get('Uri') != null;
////        },

////        setNewStream: function (streamContext, streamFilter) {
////            this.set('Context', streamContext);
////            this.set('Filter', streamFilter);
////            var uri = '';
////            if (this.has('Context')) {
////                uri = this.get('Context').get('Id');
////            }
////            //uri += '/' + this.get('filter');
////            this.set('Uri', uri);
////            this.trigger('newStream', this);
////            this.trigger('fetchingItemsStarted', this);
////            this.streamItems.fetchFirstPage(this);
////        },

////        setNewFilter: function (filter) {
////            this.set('Filter', filter);
////            this.trigger('newStreamFilter', this);
////            this.trigger('fetchingItemsStarted', this);
////            this.streamItems.fetchFirstPage(this);
////        },

////        setNextPage: function () {
////            this.trigger('newStreamPage', this);
////            this.trigger('fetchingItemsStarted', this);
////            this.streamItems.fetchNextPage(this);
////        },

////        // Add stream items manually into stream (used by notification router)
////        addStreamItem: function (streamItem) {
////            var add = false;
////            // Determine if user is currently viewing a relevant stream
////            if (this.get('Context') == null) {
////                // Home stream
////                add = true;
////            } else {
////                // Group stream
////                var self = this;
////                add = _.any(streamItem.Groups, function (groupId) {
////                    return groupId === self.get('Context').get('Id');
////                });
////            }
////            if (add) {
////                this.streamItems.add(streamItem);
////            }
////        }
//    });

//    return Stream;

//});
