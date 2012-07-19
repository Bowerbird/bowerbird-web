/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// CommentItemView
// ---------------

// Shows an individual project item
define(['jquery', 'underscore', 'backbone', 'app', 'ich', 'views/commentformview', 'models/comment'],
function ($, _, Backbone, app, ich, CommentFormView, Comment) {
    var CommentCompositeView = Backbone.Marionette.CompositeView.extend({

        tagName: 'li',

        className: 'comment-item',

        template: 'CommentItem',

        itemView: CommentCompositeView,

        events: {
            'click .reply-button': 'addReply',
            'click .save-reply-button': 'saveReply',
            'click .cancel-reply-button': 'cancelReply'
        },

        initialize: function (options) {
            //this.Comments = options.Comment.Comments;
        },

        serializeData: function () {
            return this.model.toJSON();
        },

        onRender: function () {
            log('commentCompositeView.onRender');
            this.$el.find('.reply').append(ich.ReplyForm());
        },

        addReply: function (e) {
            e.preventDefault();
            var model = new Comment({ ContributionId: this.model.get('ContributionId'), ParentCommentId: this.model.id, IsNested: true });
            this.$el.find('.reply').empty().append(ich.PostReplyForm());
        },

        saveReply: function (e) {
            var model = new Comment({
                ContributionId: this.model.get('ContributionId'),
                ParentCommentId: this.model.id,
                IsNested: true,
                Message: this.$el.find('.add-reply-text').val()
            });

            model.save();
        },

        cancelReply: function (e) {
            this.$el.find('.reply').empty().append(ich.ReplyForm());
        }
    });

    return CommentCompositeView;
});