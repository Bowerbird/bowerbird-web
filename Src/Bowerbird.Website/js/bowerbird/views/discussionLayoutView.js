/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// DiscussionLayoutView
// --------------------

define(['jquery', 'underscore', 'backbone', 'app', 'collections/commentcollection', 'models/comment', 'views/commentformview'],
function ($, _, Backbone, app, CommentCollection, Comment, CommentFormView) {
    var DiscussionLayoutView = Backbone.Marionette.ItemView.extend({

        className: 'discussion',

        template: 'Discussion',

        regions: {
            comments: '#comments',
            addcomment: '#addcomment'
        },

        events: {
            'click .add-comment-button': '_addComment'
        },

        serializeData: function () {
            return {
                Model: {
                    Comments: this.comments,
                    ContributionId: this.contributionId
                }
            };
        },

        // pass these parameters from the host 
        initialize: function (options) {
            this.contributionId = options.contributionId;
            this.comments = new CommentCollection(options.comments);
        },

        onRender: function () {

        },

        onShow: function () {
            this._showDetails();
        },

        showBootstrappedDetails: function () {
            this._showDetails();
        },

        _showDetails: function () {

        },

        _addComment: function (e) {
            e.preventDefault();
            var newComment = new Comment({ ContributionId: this.contributionId, ParentCommentId: null, IsNested: false });
            var commentFormView = new CommentFormView({ el: $('#addcomment'), model: newComment });
            commentFormView.render();
        }

    });

    return DiscussionLayoutView;

});