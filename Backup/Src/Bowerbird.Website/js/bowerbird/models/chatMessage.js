/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ChatMessage
// -----------

// Displays a message from a user in a chat window
define(['jquery', 'underscore', 'backbone', 'app'],
function ($, _, Backbone, app) 
{
    var ChatMessage = Backbone.Model.extend({
        defaults: {
            // Template has: User.Name, Message, Timestamp
        },

        idAttribute: 'Id'
    });

    return ChatMessage;
});