/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ChatView
// --------

// Shows chat window for a chat
define(['jquery', 'underscore', 'backbone', 'app', 'ich', 'views/chatmessagecollectionview', 'views/chatusercollectionview'],
function ($, _, Backbone, app, ich, ChatMessageCollectionView, ChatUserCollectionView) {
    var ChatLayoutView = Backbone.Marionette.Layout.extend({

        template: 'Chat',

        //itemView: ChatMessageItemView,

        className: 'chat-window',

        events: {
            "click .chat-send-message-button": "sendMessage",
            "click .window-close": "closeWindow"
        },

        regions: {
            messages: '.chat-messages',
            users: '.chat-users'
        },

        serializeData: function () {
            return {
                Model: {
                    Title: this.model.get('Title')
                }
            };
        },

        onShow: function () {
            log('chatView.onRender');

            var chatMessageCollectionView = new ChatMessageCollectionView({ collection: this.model.chatMessages });
            this.messages.show(chatMessageCollectionView);
            chatMessageCollectionView.render();

            var chatUserCollectionView = new ChatUserCollectionView({ collection: this.model.chatUsers });
            this.users.show(chatUserCollectionView);
            chatUserCollectionView.render();
        },

        sendMessage: function () {
            log('chatView.sendMessage');

            var message = this.$el.find('.new-chat-message').val();
            var chatId = this.model.id;
            log(message, chatId);
            app.vent.trigger('chats:sendMessage', chatId, message);
        },

        closeWindow: function () {
            app.chats.remove(this.model.id);
            this.$el.remove();
        }
    });

    return ChatLayoutView;
})