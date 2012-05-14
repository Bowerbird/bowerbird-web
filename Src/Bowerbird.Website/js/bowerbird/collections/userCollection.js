/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// UserCollection
// --------------

define(['jquery', 'underscore', 'backbone', 'app', 'models/user'], function ($, _, Backbone, app, User) {

    var UserCollection = Backbone.Collection.extend({
        model: User,

        url: '/users',

        initialize: function () {
            _.extend(this, Backbone.Events);
            _.bindAll(this, 'updateUserStatus');
        },

        updateUserStatus: function (newUser) {
            if (!this.contains(newUser.Id)) {
                if (newUser.Status == 2 || newUser.Status == 3 || newUser.Status == 'undefined') return;
                var user = new User(newUser);
                app.onlineUsers.add(user);
            } else {
                var user = this.get(newUser.Id);
                if (newUser.Status == 2 || newUser.Status == 3) {
                    app.onlineUsers.remove(user);
                } else {
                    user.set('Status', newUser.Status);
                }
            }
        }
    });

    return UserCollection;

});