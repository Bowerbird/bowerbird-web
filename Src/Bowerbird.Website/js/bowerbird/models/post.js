/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// Post
// ----

define(['jquery', 'underscore', 'backbone'], function ($, _, Backbone) {

    var Post = Backbone.Model.extend({
        defaults: {
            Subject: '',
            Message: '',
            GroupId: null
        },

        idAttribute: 'Id',

        urlRoot: '/posts',

        toJSON: function () {
            return {
                Subject: this.get('Subject'),
                Message: this.get('Message'),
                GroupId: this.get('GroupId')
            };
        }

    });

    return Post;

});