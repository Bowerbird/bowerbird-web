/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ProjectItemView
// ---------------

// Shows an individual project item
define(['jquery', 'underscore', 'backbone', 'app'],
function ($, _, Backbone, app) {
    var ProjectItemView = Backbone.Marionette.ItemView.extend({

        tagName: 'li',

        className: 'explore-project-item',

        template: 'ProjectItem',

        events: {
            'click .join-project-button': 'joinProject',
            'click .leave-project-button': 'leaveProject'
        },

        serializeData: function () {
            return {
                Model: {
                    Project: this.model.toJSON(),
                    IsMember: _.any(app.authenticatedUser.memberships, function (membership) { return membership.GroupId === this.model.id; }, this),
                    MemberCountDescription: this.model.get('MemberCount') === 1 ? 'Member' : 'Members',
                    ObservationCountDescription: this.model.get('ObservationCount') === 1 ? 'Observation' : 'Observations',
                    PostCountDescription: this.model.get('PostCount') === 1 ? 'Post' : 'Posts'
                }
            };
        },

        joinProject: function (e) {
            e.preventDefault();
            app.vent.trigger('joinProject', this.model);
            this.$el.find('.join-project-button').replaceWith('<a href="#" class="leave-project-button button">Leave Project</a>');
        },

        leaveProject: function (e) {
            e.preventDefault();
            app.vent.trigger('leaveProject', this.model);
            this.$el.find('.leave-project-button').replaceWith('<a href="#" class="join-project-button button">Join Project</a>');
        }
    });

    return ProjectItemView;
});