/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// UserCollection
// --------------

define(['jquery', 'underscore', 'backbone', 'models/user'], function ($, _, Backbone, User) {

    var UserCollection = Backbone.Collection.extend({
        model: User,

        url: '/users',

        initialize: function () {
            _.extend(this, Backbone.Events);
        },

        comparator: function (user) {
            return user.get('Name');
        }
    });

    return UserCollection;

});