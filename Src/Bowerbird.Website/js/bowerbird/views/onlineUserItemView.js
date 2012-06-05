/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// OnlineUserItemView
// ------------------

define(['jquery', 'underscore', 'backbone', 'app', 'models/user'], function ($, _, Backbone, app, User) {

    var OnlineUserItemView = Backbone.Marionette.ItemView.extend({
        
        tagName: 'li',

        className: 'online-user',

        template: 'OnlineUserItem',

        events: {
        },

        onRender: function () {
            
        },

        serializeData: function () {
            return {
                Id: this.model.id,
                Name: this.model.get('Name'),
                Avatar: this.model.get('Avatar'),
                Type: 'User'
            };
        }
    });

    return OnlineUserItemView;

});
