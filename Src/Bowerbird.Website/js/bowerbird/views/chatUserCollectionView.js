/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ChatUserCollectionView
// ----------------------

// Shows users in a chat
define(['jquery', 'underscore', 'backbone', 'app', 'ich', 'collections/usercollection', 'views/useritemview'],
function ($, _, Backbone, app, ich, UserCollection, UserItemView) 
{
    var ChatUserCollectionView = Backbone.Marionette.CollectionView.extend({

        itemView: UserItemView,

        template: 'UserItem',

        serializeData: function () {
            return this.model.toJson();
        }

    });

    return ChatUserCollectionView;
})