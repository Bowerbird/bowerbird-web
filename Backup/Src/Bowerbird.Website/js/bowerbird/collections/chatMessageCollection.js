/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ChatMessageCollection
// ---------------------

define(['jquery', 'underscore', 'backbone', 'models/chatmessage'],
function ($, _, Backbone, ChatMessage) 
{
    var ChatMessageCollection = Backbone.Collection.extend({
        
        model: ChatMessage,

        url: '/chatmessages',

        initialize: function () {
            _.extend(this, Backbone.Events);
        }
    });

    return ChatMessageCollection;
});