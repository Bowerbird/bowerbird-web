/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ProjectDetailsView
// ------------------

define(['jquery', 'underscore', 'backbone', 'app', 'views/activitylistview', 'collections/activitycollection'], function ($, _, Backbone, app, ActivityListView, ActivityCollection) {

    var ProjectDetailsView = Backbone.Marionette.Layout.extend({
        viewType: 'details',

        className: 'project double',

        template: 'Project',

        regions: {
            details: '.details'
        },

        serializeData: function () {
            return {
                Model: {
                    Project: this.model.toJSON(),
                    IsMember: _.any(app.authenticatedUser.memberships, function (membership) { return membership.GroupId === this.model.id; }, this),
                    MemberCountDescription: this.model.get('MemberCount') === 1 ? 'Member' : 'Members',
                    ObservationCountDescription: this.model.get('ObservationCount') === 1 ? 'Sighting' : 'Sightings',
                    PostCountDescription: this.model.get('PostCount') === 1 ? 'Post' : 'Posts'
                }
            };
        },

        showBootstrappedDetails: function () {
            this.initializeRegions();
            this.$el = $('#content .project');
        },

        showActivity: function () {
            var activityCollection = new ActivityCollection(null, { groupOrUser: this.model });
            var options = {
                model: this.model,
                collection: activityCollection
            };
            if (app.isPrerenderingView('projects')) {
                options['el'] = '.stream';
            }
            var activityListView = new ActivityListView(options);
            if (app.isPrerenderingView('projects')) {
                this.details.attachView(activityListView);
                activityListView.showBootstrappedDetails();
            } else {
                this.details.show(activityListView);
            }
            activityCollection.fetchFirstPage();
        }
    });

    return ProjectDetailsView;

}); 