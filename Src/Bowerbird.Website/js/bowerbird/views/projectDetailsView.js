/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ProjectDetailsView
// ------------------

define(['jquery', 'underscore', 'backbone', 'app', 'views/activitylistview', 'views/sightinglistview', 'views/postlistview', 'views/userlistview', 'views/projectaboutview'],
function ($, _, Backbone, app, ActivityListView, SightingListView, PostListView, UserListView, ProjectAboutView) {

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
            'click .posts-tab-button': 'showPostsTabSelection',
            'click .members-tab-button': 'showMembersTabSelection',
            'click .about-tab-button': 'showAboutTabSelection'
        },

        activeTab: '',

        serializeData: function () {
            return {
                Model: {
                    Project: this.model.toJSON(),
                    IsMember: _.any(app.authenticatedUser.memberships, function (membership) { return membership.GroupId === this.model.id; }, this),
                    MemberCountDescription: this.model.get('MemberCount') === 1 ? 'Member' : 'Members',
                    SightingCountDescription: this.model.get('SightingCount') === 1 ? 'Sighting' : 'Sightings',
                    PostCountDescription: this.model.get('PostCount') === 1 ? 'Post' : 'Posts'
                }
            };
        },

        showBootstrappedDetails: function () {
            this.initializeRegions();
        },

        showActivityTabSelection: function (e) {
            this.changeTab(e, 'activities');
        },

        showSightingsTabSelection: function (e) {
            this.changeTab(e, 'sightings');
        },

        showPostsTabSelection: function (e) {
            this.changeTab(e, 'posts');
        },

        showMembersTabSelection: function (e) {
            this.changeTab(e, 'members');
        },

        showAboutTabSelection: function (e) {
            this.changeTab(e, 'about');
        },

        changeTab: function (e, name) {
            e.preventDefault();
            this.switchTabHighlight(name);
            if (this.list.currentView.showLoading) {
                this.list.currentView.showLoading();
            }
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

        showPosts: function (postCollection) {
            this.switchTabHighlight('posts');

            var options = {
                model: this.model,
                collection: postCollection
            };

            if (app.isPrerenderingView('projects')) {
                options['el'] = '.list > div';
            }

            var postListView = new PostListView(options);

            if (app.isPrerenderingView('projects')) {
                this.list.attachView(postListView);
                postListView.showBootstrappedDetails();
            } else {
                this.list.show(postListView);
            }
        },

        showMembers: function (userCollection) {
            this.switchTabHighlight('members');

            var options = {
                model: this.model,
                collection: userCollection
            };

            if (app.isPrerenderingView('projects')) {
                options['el'] = '.list > div';
            }

            var userListView = new UserListView(options);

            if (app.isPrerenderingView('projects')) {
                this.list.attachView(userListView);
                userListView.showBootstrappedDetails();
            } else {
                this.list.show(userListView);
            }
        },

        showAbout: function (projectAdministrators, activityTimeseries) {
            this.switchTabHighlight('about');

            var options = {
                model: this.model
            };

            if (app.isPrerenderingView('projects')) {
                options['el'] = '.list > div';
            }

            options.projectAdministrators = projectAdministrators;
            options.activityTimeseries = activityTimeseries;

            var projectAboutView = new ProjectAboutView(options);

            if (app.isPrerenderingView('projects')) {
                this.list.attachView(projectAboutView);
                projectAboutView.showBootstrappedDetails();
            } else {
                this.list.show(projectAboutView);
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