/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ChatMessageItemView
// -------------------

// Shows a message from a user in a chat window
define(['jquery', 'underscore', 'backbone', 'app', 'models/chatmessage', 'date'],
function ($, _, Backbone, app) {
    var ChatMessageItemView = Backbone.Marionette.ItemView.extend({

        tagName: 'li',

        className: 'chat-message',

        template: 'ChatMessage',

        serializeData: function () {
            var model = null;
            if (this.model.get('Type') === 'usermessage') {
                var timestamp = this.model.get('Timestamp');
                model = {
                    UserMessage: {
                        From: app.authenticatedUser.user.get('Name') === this.model.get('FromUser').Name ? 'me' : this.model.get('FromUser').Name,
                        Message: this.model.get('Message').replace(/\n/g, '<br />'),
                        Time: timestamp === '' || timestamp == null ? '' : new Date(timestamp).toString('hh:mmtt'),
                        Timestamp: new Date(this.model.get('Timestamp')).toString()
                    }
                };
            }
            if (this.model.get('Type') === 'useradded') {
                model = {
                    UserAdded: {
                        Message: this.model.get('Message')
                    }
                };
            }
            return {
                Model: model
            };
        },

        onRender: function () {
            this.$el.addClass(this.model.get('Type'));
            app.vent.trigger('chats:itemview:added', this);
            this.model.on('change:Timestamp', this.updateTimestamp, this);
        },

        updateTimestamp: function (model) {
            log('timestamp updated', model);
            var timestamp = model.get('Timestamp');
            if (timestamp != null && timestamp != '') {
                this.$el.find('.timestamp').text(new Date(timestamp).toString('hh:mmtt'));
            }
        }
    });

    return ChatMessageItemView;
});