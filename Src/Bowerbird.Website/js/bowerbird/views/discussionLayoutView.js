/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// DiscussionLayoutView
// --------------------

define(['jquery', 'underscore', 'backbone', 'app', 'collections/commentcollection', 'models/comment', 'views/commentformview', 'views/commentcollectionview'],
function ($, _, Backbone, app, CommentCollection, Comment, CommentFormView, CommentCollectionView) {
    var DiscussionLayoutView = Backbone.Marionette.Layout.extend({

        className: 'discussion',

        template: 'Discussion',

        regions: {
            comments: '#comments',
            addcomment: '#addcomment'
        },

        events: {
            'click .add-comment-button': '_addComment',
            'commentcancelled:': '_addNewCommentButton'
        },

        serializeData: function () {
            return {
                Model: {
                    Comments: this.Comments.toJSON(),
                    ContributionId: this.ContributionId
                }
            };
        },

        // pass these parameters from the host 
        initialize: function (options) {
            log('discussionLayoutView.initialize');
            _.bindAll(this, 'onRender', 'commentActivityReceived');
            this.ContributionId = options.ContributionId;
            this.Comments = new CommentCollection(options.Comments);
        },

        onRender: function () {
            log('discussionLayoutView.onRender');
            var commentCollectionView = new CommentCollectionView({ collection: this.Comments });
            this.comments.show(commentCollectionView);
            commentCollectionView.render();

            app.vent.on('newactivity:postcommentadded:' + this.ContributionId + ' newactivity:observationcommentadded:' + this.ContributionId, this.commentActivityReceived, this);
        },

        onShow: function () {
            log('discussionLayoutView.onShow');
        },

        showBootstrappedDetails: function () {
            log('discussionLayoutView.showBootstrappedDetails');
            this._showDetails();
        },

        _showDetails: function () {
        },

        _addComment: function (e) {
            e.preventDefault();
            var newComment = new Comment({ ContributionId: this.ContributionId, ParentCommentId: null, IsNested: false });
            var commentFormView = new CommentFormView({ el: $('#addcomment'), model: newComment });
            commentFormView.render();
        },

        _addNewCommentButton: function () {
            this.$el.find('#addcomment').append('<input class="add-comment-button" type="button" value="add-new-comment">');
        },

        commentActivityReceived: function (commentActivity) {
            if (commentActivity.get('Type') == "postcommentadded") {
                var comment = new Comment(commentActivity.get('PostCommentAdded').Comment);
                this.Comments.add(comment);
            } else if (commentActivity.get('Type') == "observationcommentadded") {
                var comment = new Comment(commentActivity.get('ObservationCommentAdded').Comment);
                this.Comments.add(comment);
            }
        }

    });

    return DiscussionLayoutView;

});