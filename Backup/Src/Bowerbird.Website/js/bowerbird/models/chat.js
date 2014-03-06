/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// Chat
// ----

define(['jquery', 'underscore', 'backbone', 'app', 'collections/usercollection', 'collections/chatmessagecollection', 'models/user', 'models/chatmessage'],
function ($, _, Backbone, app, UserCollection, ChatMessageCollection, User, ChatMessage) {

    var Chat = Backbone.Model.extend({
        defaults: {
            IsStarted: false,
            Group: null // If group chat, this must be set
        },

        idAttribute: 'Id',

        initialize: function (attributes, options) {
            this.chatUsers = options.chatUsers;
            this.chatMessages = options.chatMessages;
        },

        chatType: function () {
            if (this.get('Group') != null) {
                return 'group';
            }
            return 'private';
        },

        // Call this method once chat is setup and user can start sending messages
        start: function (chatDetails) {
            if (chatDetails) {
                this.chatUsers.reset(chatDetails.Users);
                this.chatMessages.reset(chatDetails.Messages);
            }
            log('chat started', this.id);
            this.set('IsStarted', true);
        }
    });

    return Chat;

});