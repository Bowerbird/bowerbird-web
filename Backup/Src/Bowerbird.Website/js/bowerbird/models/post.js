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
            Id: null,
            Subject: '',
            Message: '',
            PostType: '',
            GroupId: null
        },

        url: function () {
            var url = '/' + this.get('GroupId');
            if (this.id) {
                url += '/' + this.id;
            } else {
                url += '/posts';
            }
            return url;
        },

        idAttribute: 'Id',

        toJSON: function () {
            return {
                Id: this.id,
                Subject: this.get('Subject'),
                Message: this.get('Message'),
                PostType: this.get('PostType')
            };
        },
        
        toViewJSON: function() {
            return {
                Id: this.id,
                Subject: this.get('Subject'),
                Message: this.get('Message'),
                PostType: this.get('PostType'),
                Group: this.get('Group'),
                User: this.get('User'),
                CreatedOnDescription: this.get('CreatedOnDescription')
            };
        }
    });

    return Post;

});