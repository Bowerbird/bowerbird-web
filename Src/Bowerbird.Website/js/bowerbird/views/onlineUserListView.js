 /// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// OnlineUserListView
// ------------------

define(['jquery', 'underscore', 'backbone', 'app', 'views/onlineuseritemview'],
function ($, _, Backbone, app, OnlineUserItemView) {

    var OnlineUserListView = Backbone.Marionette.CompositeView.extend({

        itemView: OnlineUserItemView,

        template: 'OnlineUserList',

        id: 'online-users',

        className: 'window collapsed',

        events: {
            'click .window-title-bar': 'toggleCollapsed'
        },

        initialize: function (options) {
            this.collection.on('add', this.onAdd, this);
            this.collection.on('remove', this.onRemove, this);
        },

        appendHtml: function (collectionView, itemView) {
            var index = collectionView.collection.indexOf(itemView.model);
            if (index === 0 && collectionView.$el.find('.online-users-list > li').length === 0) {
                collectionView.$el.find('.online-users-list').append(itemView.el);
            } else {
                collectionView.$el.find('.online-users-list > li').eq(index - 1).after(itemView.el);
            }
        },

        onShow: function () {
            $('body').append(this.el);
        },

        serializeData: function () {
            return {
                Model: {
                    Count: this.collection.length,
                    TitleDescription: this.getTitle(this.collection.length)
                }
            };
        },

        onAdd: function (model, collection) {
            model.on('statuschange', this.onStatusChange, this);
            this.updateUserCount(collection);
        },

        onRemove: function (model, collection) {
            model.off('statuschange');
            this.updateUserCount(collection);
        },

        toggleCollapsed: function () {
            this.$el.toggleClass('collapsed');
        },

        getTitle: function (count) {
            return (count == 1 ? 'Person' : 'People') + ' Online';
        },

        onStatusChange: function (user) {
            this.updateUserCount(this.collection);
        },

        updateUserCount: function (collection) {
            var count = 0;
            collection.each(function (user) {
                var currentStatus = user.getCurrentStatus();
                if (currentStatus === 'online' || currentStatus === 'away') {
                    count++;
                }
            });

            this.$el.find('.user-count').text(count);
            this.$el.find('.title-description').text(this.getTitle(count));
        }
    });

    app.addInitializer(function (options) {
        $(function () {
            if (app.authenticatedUser) {
                var onlineUserListView = new OnlineUserListView({ model: app.onlineUsers, collection: app.onlineUsers });

                onlineUserListView.on('show', function () {
                    app.vent.trigger('onlineUsers:rendered');
                });

                app.usersonline.show(onlineUserListView);
            }
        });
    });

    return OnlineUserListView;

});