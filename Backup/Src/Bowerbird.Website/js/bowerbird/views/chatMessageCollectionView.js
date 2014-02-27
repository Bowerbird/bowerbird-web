/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ChatMessageCollectionView
// -------------------------

// Shows messages in a chat
define(['jquery', 'underscore', 'backbone', 'app', 'ich', 'views/chatmessageitemview'],
function ($, _, Backbone, app, ich, ChatMessageItemView) {
    var ChatMessageCollectionView = Backbone.Marionette.CollectionView.extend({
        tagName: 'ul',

        itemView: ChatMessageItemView
    });

    return ChatMessageCollectionView;
})