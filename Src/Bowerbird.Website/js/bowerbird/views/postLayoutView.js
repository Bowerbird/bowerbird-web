/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ObservationLayoutView
// ---------------------

// Layout of an observation in both edit and view mode
define(['jquery', 'underscore', 'backbone', 'app', 'views/postdetailsview', 'views/postformlayoutview', 'views/discussionlayoutview'],
function ($, _, Backbone, app, PostDetailsView, PostFormLayoutView, DiscussionLayoutView) 
{
    var PostLayoutView = Backbone.Marionette.Layout.extend({
        className: 'post',

        template: 'Post',

        regions: {
            main: '.main',
            notes: '.notes',
            comments: '.comments-details'
        },

        events: {
            
        },

        showBootstrappedDetails: function () {
            this.initializeRegions();
            this.$el = $('#content .post');
        },

        showPostDetails: function (post) {
            var options = { model: post };

            if (app.isPrerendering('posts')) {
                options['el'] = '.post-details';
            }

            var postDetailsView = new PostDetailsView(options);
            this.main[app.getShowViewMethodName('posts')](postDetailsView);

            var discussionLayoutView = new DiscussionLayoutView({ Comments: post.get('Comments'), ContributionId: post.id });
            this.comments[app.getShowViewMethodName('posts')](discussionLayoutView);

            if (app.isPrerendering('posts')) {
                postDetailsView.showBootstrappedDetails();
            }
        },

        showPostDiscussion: function (post) {
            var options = { comments: post.get('Comments'), contributionId: post.id };

            if (app.isPrerendering('posts')) {
                options['el'] = '.discussion';
            }

            var discussionLayoutView = new DiscussionLayoutView(options);
            this.comments[app.getShowViewMethodName('posts')](discussionLayoutView);

            if (app.isPrerendering('posts')) {
                discussionLayoutView.showBootstrappedDetails();
            }
        },

        showPostForm: function (post) {
            var options = { model: post };

            if (app.isPrerendering('posts')) {
                options['el'] = '.post-form';
            }

            var postFormLayoutView = new PostFormLayoutView(options);
            this.main[app.getShowViewMethodName('posts')](postFormLayoutView);

            if (app.isPrerendering('posts')) {
                postFormLayoutView.showBootstrappedDetails();
            }
        }
    });

    return PostLayoutView;

});