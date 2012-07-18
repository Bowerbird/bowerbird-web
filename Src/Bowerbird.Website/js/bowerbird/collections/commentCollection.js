/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// CommentCollection
// -----------------

define(['jquery', 'underscore', 'backbone', 'models/comment'],
function ($, _, Backbone, Comment) 
{
    var CommentCollection = Backbone.Collection.extend({
        
        model: Comment,

        url: '/comments',

        initialize: function () {
            _.extend(this, Backbone.Events);
        }
    });

    return CommentCollection;
});