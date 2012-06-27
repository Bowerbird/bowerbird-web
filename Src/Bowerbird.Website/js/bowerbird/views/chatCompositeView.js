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

        initialize: function (options) {
        },

        serializeData: function () {
            var title = '';
            if (this.model.chatType() == 'private') {
                title = _.without(this.model.chatUsers.pluck('Name'), app.authenticatedUser.user.get('Name')).join(', ');
            } else {
                title = this.model.get('Group').get('Name');
            }
            return {
                Model: {
                    Title: title
                }
            };
        },

        onShow: function () {
            log('chatView.onRender');

            this.$el.find('.chat-send-message-button').attr("disabled", true);
            this.model.on('change:IsStarted', this.onChatStarted, this);

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
                    if (that.model.get('IsStarted') === true) {
                        that.sendMessage.call(that);
                    }
                }
            });

            this.wasTyping = false;

            this.$el.find('.new-chat-message').keypress(function () {
                if (that.model.get('IsStarted') === true) {
                    var isTyping = false;
                    if ($(this).val().length > 0) {
                        isTyping = true;
                    }
                    if (that.wasTyping != isTyping) {
                        app.vent.trigger('chats:useristyping', that.model, isTyping);
                    }
                    that.wasTyping = isTyping;
                }
            });

            this.model.chatUsers.on('change:IsTyping', this.updateTypingStatus, this);

            this.model.chatMessages.on('add', this.onChatMessageAdded, this);
        },

        onChatStarted: function () {
            // When chat starts, enable all buttons and add the auth user to the view
            this.$el.find('.chat-send-message-button').attr("disabled", false);
            this.user = this.model.chatUsers.get(app.authenticatedUser.user.id);
        },

        sendMessage: function () {
            var $messageTextArea = this.$el.find('.new-chat-message');
            var message = $.trim($messageTextArea.val());
            if (message.length > 0) {
                app.vent.trigger('chats:sendMessage', this.model, message);
                $messageTextArea.val('').focus();
            }
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
        },

        onChatMessageAdded: function (message) {
            if (message.get('Type') === 'usermessage') {
                var user = this.model.chatUsers.get(message.get('FromUser').Id).set('IsTyping', false);
                this.updateTypingStatus();
            }
        },

        updateTypingStatus: function () {
            if (this.model.get('IsStarted') === true) {
                var users = _.reject(this.model.chatUsers.where({ 'IsTyping': true }), function (u) { return u.id == this.user.id; }, this);
                var names = _.map(users, function (user) { return user.get('Name'); }, this);

                var desc = '';
                if (names.length == 1) {
                    desc = names[0] + ' is typing...';
                } else if (names.length > 1) {
                    desc = names.join(', ') + ' are typing...';
                }

                this.$el.find('.status').text(desc);
            }
        }
    });

    return ChatLayoutView;
})