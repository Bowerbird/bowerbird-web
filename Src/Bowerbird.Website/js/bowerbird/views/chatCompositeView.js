/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ChatView
// --------

// Shows chat window for a chat
define(['jquery', 'underscore', 'backbone', 'app', 'ich', 'views/chatmessagecollectionview'],
function ($, _, Backbone, app, ich, ChatMessageCollectionView, ChatUserCollectionView) {
    var ChatLayoutView = Backbone.Marionette.Layout.extend({
        className: 'window chat',

        template: 'Chat',

        events: {
            "click .chat-send-message-button": "sendMessage",
            'click .window-title-bar': 'toggleCollapsed',
            'click .window-title-bar .close': 'closeWindow'
        },

        regions: {
            messages: '.chat-messages'
        },

        serializeData: function () {
            var title = '';
            if (this.model.chatType() == 'private') {
                title = _.without(this.model.chatUsers.pluck('Name'), app.authenticatedUser.user.get('Name')).join(', ');
            } else {
                title = this.model.get('Group').Name;
            }
            return {
                Model: {
                    Title: title
                }
            };
        },

        onShow: function () {
            log('chatView.onRender');

            var left = (app.chats.length * 300) + 20; // (num of chats * width of a chat) + some space between online users
            this.$el.css({ left: left });
            $('body').append(this.el);
            this.$el.find('.new-chat-message').focus();

            var chatMessageCollectionView = new ChatMessageCollectionView({ collection: this.model.chatMessages });
            this.messages.show(chatMessageCollectionView);
            chatMessageCollectionView.render();

            chatMessageCollectionView.on('item:afteradd', this.onItemViewAdded, this);
            var that = this;
            this.$el.find('.new-chat-message').keydown(function (e) {
                // Enter was pressed without shift key
                if (e.keyCode == 13 && !e.shiftKey) {
                    // prevent default behavior
                    e.preventDefault();
                    // Send the message
                    that.sendMessage();
                }
            });
        },

        sendMessage: function () {
            var message = this.$el.find('.new-chat-message').val();
            app.vent.trigger('chats:sendMessage', this.model, message);
            this.$el.find('.new-chat-message').val('').focus();
        },

        closeWindow: function () {
            app.vent.trigger('chats:close', this.model);
        },

        toggleCollapsed: function () {
            this.$el.toggleClass('collapsed');
        },

        onItemViewAdded: function (view) {
            var height = this.$el.find('.chat-messages')[0].scrollHeight;
            this.$el.find('.chat-messages').scrollTop(height);
        }
    });

    return ChatLayoutView;
})