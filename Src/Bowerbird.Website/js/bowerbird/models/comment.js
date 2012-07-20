/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// Comment
// -------

define(['jquery', 'underscore', 'backbone'],
function ($, _, Backbone) {
    var Comment = Backbone.Model.extend({
        defaults: {
            Message: '',
            ContributionId: null,
            ParentCommentId: null,
            IsNested: false,
            Comments: []
        },

        idAttribute: 'Id',

        urlRoot: '/comments',

        toJSON: function () {
            return {
                Subject: this.get('Subject'),
                Message: this.get('Message'),
                ContributionId: this.get('ContributionId'),
                ParentCommentId: this.get('ParentCommentId'),
                IsNested: this.get('IsNested')
                //Comments: this.get('Comment').Comments.toJSON()
            };
        },

        // keep recursing through Comments collection until comment is found to add to
        addNestedComment: function (comment, commentIdTokens) {
            log('comment.addNestedComment', comment, commentIdTokens);
            // find the comment in Comments to pass comment down to
            if (commentIdTokens.length > 1) 
            {
                // pop the next id off the array, find the comment at that index (minus 1 as naming starts at 1)
                var index = commentIdTokens.pop();
                var commentToAddTo = this.get('Comments')[index - 1];
                commentToAddTo.addNestedComment(comment);
            }
            else 
            {
                // we are adding the comment to this comments collection
                Comments.add(comment);
            }
        }

    });

    return Comment;

});