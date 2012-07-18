/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// Comment
// -------

define(['jquery', 'underscore', 'backbone'],
function ($, _, Backbone) 
{
    var Comment = Backbone.Model.extend({
        defaults: {
            Message: '',
            ContributionId: null,
            ParentCommentId: null,
            IsNested: false
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
            };
        }

    });

    return Comment;

});