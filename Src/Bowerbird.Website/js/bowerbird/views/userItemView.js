/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// OnlineUserItemView
// ------------------

define(['jquery', 'underscore', 'backbone', 'app', 'models/user'], function ($, _, Backbone, app) {

    var UserItemView = Backbone.Marionette.ItemView.extend({

        tagName: 'li',

        className: 'online-user-item',

        template: 'UserItem',

        events: {
            'click .online-user-item a': 'startChat'
        },

        startChat: function (e) {
            app.vent.trigger('chats:startPrivateChat', this.model);
        }

    });

    return UserItemView;

});
