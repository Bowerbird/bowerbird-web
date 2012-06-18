/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ChatMessageItemView
// -------------------

// Shows a message from a user in a chat window
define(['jquery', 'underscore', 'backbone', 'app', 'models/chatmessage'],
function ($, _, Backbone, app) 
{
    var ChatMessageItemView = Backbone.Marionette.ItemView.extend({

        tagName: 'li',

        className: 'chat-message',

        template: 'ChatMessage',

        serializeData: function () {
            return {
                Model: this.model.toJSON()
            };
        }
    });

    return ChatMessageItemView;
});