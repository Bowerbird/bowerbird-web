/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ChatUserItemView
// ----------------

// Shows a user in a list of participating users within a chat
define(['jquery', 'underscore', 'backbone', 'app', 'models/user'],
function ($, _, Backbone, app) 
{
    var ChatUserItemView = Backbone.Marionette.ItemView.extend({

        tagName: 'li',

        className: 'chat-user',

        template: 'UserItem',

        serializeData: function () {
            return {
                Id: this.model.id,
                Name: this.model.get('Name'),
                Avatar: this.model.get('Avatar'),
                Type: 'User'
            };
        }
    });

    return ChatUserItemView;
});