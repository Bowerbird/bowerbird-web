/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ProjectDetailsView
// ------------------

define(['jquery', 'underscore', 'backbone', 'app', 'views/activitylistview', 'views/sightinglistview'],
function ($, _, Backbone, app, ActivityListView, SightingListView) {

    var ProjectDetailsView = Backbone.Marionette.Layout.extend({
        viewType: 'detail',

        className: 'project double',

        template: 'Project',

        regions: {
            summary: '.summary',
            list: '.list'
        },

        events: {
            'click .activities-tab-button': 'showActivityTabSelection',
            'click .sightings-tab-button': 'showSightingsTabSelection',
            'click .posts-tab-button': 'showPostsTabSelection'
        },

        activeTab: '',

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
            //this.$el = $('#content .project');
        },

        showActivityTabSelection: function (e) {
            e.preventDefault();
            this.switchTabHighlight('activities');
            this.list.currentView.showLoading();
            Backbone.history.navigate($(e.currentTarget).attr('href'), { trigger: true });
        },

        showSightingsTabSelection: function (e) {
            e.preventDefault();
            this.switchTabHighlight('sightings');
            this.list.currentView.showLoading();
            Backbone.history.navigate($(e.currentTarget).attr('href'), { trigger: true });
        },

        showPostsTabSelection: function (e) {
            e.preventDefault();
            this.switchTabHighlight('posts');
            this.list.currentView.showLoading();
            Backbone.history.navigate($(e.currentTarget).attr('href'), { trigger: true });
        },

        showActivity: function (activityCollection) {
            this.switchTabHighlight('activities');

            var options = {
                model: this.model,
                collection: activityCollection
            };

            if (app.isPrerenderingView('projects')) {
                options['el'] = '.list > div';
            }

            var activityListView = new ActivityListView(options);

            if (app.isPrerenderingView('projects')) {
                this.list.attachView(activityListView);
                activityListView.showBootstrappedDetails();
            } else {
                this.list.show(activityListView);
            }
        },

        showSightings: function (sightingCollection) {
            this.switchTabHighlight('sightings');

            var options = {
                model: this.model,
                collection: sightingCollection
            };

            if (app.isPrerenderingView('projects')) {
                options['el'] = '.list > div';
            }

            var sightingListView = new SightingListView(options);

            if (app.isPrerenderingView('projects')) {
                this.list.attachView(sightingListView);
                sightingListView.showBootstrappedDetails();
            } else {
                this.list.show(sightingListView);
            }
        },
        
        switchTabHighlight: function (tab) {
            this.activeTab = tab;
            this.$el.find('.tab-button').removeClass('selected');
            this.$el.find('.' + tab + '-tab-button').addClass('selected');
        }
    });

    return ProjectDetailsView;

}); 