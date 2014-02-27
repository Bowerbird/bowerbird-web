/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// CommentCollection
// -----------------

define(['jquery', 'underscore', 'backbone', 'models/comment'],
function ($, _, Backbone, Comment) {
    var CommentCollection = Backbone.Collection.extend({

        model: Comment,

        url: '/comments',

        initialize: function () {
            _.extend(this, Backbone.Events);
        },

        // find the actual comment within the collection, possibly with nested comments..
        // this is done by taking the id of the comment, and finding it's parent
        addComment: function (comment) {
            log('commentCollection.addComment', comment);
            if (comment.IsNested) {
                var commentIdTokens = comment.Id.split('.').reverse();
                var commentToAddTo = this.at(commentIdTokens.pop() - 1);
                commentToAddTo.AddNestedComment(comment, commentIdTokens);
            }
            else {
                this.add(comment);
            }
        }
    });

    return CommentCollection;
});