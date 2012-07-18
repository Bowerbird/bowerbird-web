/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// CommentItemView
// ---------------

// Shows an individual project item
define(['jquery', 'underscore', 'backbone', 'app', 'views/commentformview', 'models/comment'], 
function ($, _, Backbone, app, CommentFormView, Comment) 
{
    var CommentItemView = Backbone.Marionette.ItemView.extend({

        tagName: 'li',

        className: 'comment-item',

        template: 'CommentItem',

        events: {
            'click .add-comment-button': 'addComment'
        },

        serializeData: function () {
            return this.model.toJSON();
        },

        addComment: function (e) {
            e.preventDefault();
            // we are essentially creating a new comment model, where: 
            // ParentCommentId is this model's id.
            // ContributionId is this model's contributionId.
            // IsNested = true.
            // Message is blank.
            
            // create a new comment form, pass in the model and display.
            var model = new Comment({ContributionId: this.model.get('ContributionId'), ParentCommentId: this.model.id, IsNested: true });
            var view = new CommentFormView({ model: model });
            this.$el.find('.add-a-comment').append(view.render());
        }
    });

    return CommentItemView;
});