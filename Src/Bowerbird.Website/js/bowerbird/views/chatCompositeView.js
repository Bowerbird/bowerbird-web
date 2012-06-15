/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ChatView
// --------

// Shows chat window for a chat
define(['jquery', 'underscore', 'backbone', 'app', 'ich', 'views/chatmessageitemview', 'views/chatusercollectionview', 'views/useritemview'],
function ($, _, Backbone, app, ich, ChatMessageItemView, ChatUserCollectionView, UserItemView) {
    var ChatCompositeView = Backbone.Marionette.CompositeView.extend({

        template: 'Chat',

        itemView: ChatMessageItemView,

        className: 'chat-window',

        events: {
            "click .chat-send-message-button": "sendMessage",
            "click .window-close": "closeWindow"
        },

        regions: {
            users: '#chat-users',
            messages: '.chat-messages'
        },

        //        serializeData: function () {
        //            log('chatView.serializeData');
        //            var model = this.model.toJSON();

        //            return {
        //                Model: model
        //            };
        //        },

//        render: function(){
//            log('chatView.render');
//        },

        onRender: function () {
            log('chatView.onRender');

            $('body').append(this.el);

            var chatUserCollectionView = new ChatUserCollectionView({ model: this.model.ChatUsers });
            chatUserCollectionView.itemView = UserItemView;
            this.users.appendView(chatUserCollectionView);



            //app.vent.on('newmessage:' + this.ChatId, this.addChatMessage, this);
        },

        sendMessage: function () {
            log('chatView.sendMessage');
            //app.chatRouter.sendMessage(this.$el.find('.new-chat-message').val(), this.chat);
            var message = this.$el.find('.new-chat-message').val();
            var chatId = this.model.get('ChatId');
            log(message, chatId);
            app.vent.trigger('chats:sendMessage', chatId, message);
        },

        closeWindow: function () {
            app.chats.remove(this.chat);
            this.$el.remove();
        },

        addChatMessage: function (chatMessage) {
            log('chatView.addChatMessage', chatMessage);
            this.$el.find('.chat-messages').append(ich.ChatMessage(chatMessage.toJSON()));

        }

    });

    return ChatCompositeView;
})