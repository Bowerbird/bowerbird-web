/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// OrganisationAboutView
// ----------------

define(['jquery', 'underscore', 'backbone', 'app', 'visualize'], function ($, _, Backbone, app) {

    var OrganisationAboutView = Backbone.Marionette.ItemView.extend({
        //className: 'organisation-about',

        template: 'OrganisationAbout',

        initialize: function (options) {
            _.bindAll(this, 'refresh');
            this.organisationAdministrators = options.organisationAdministrators;
            this.activityTimeseries = options.activityTimeseries;
        },

        serializeData: function () {
            return {
                Model: {
                    Organisation: this.model.toJSON(),
                    IsMember: _.any(app.authenticatedUser.memberships, function (membership) { return membership.GroupId === this.model.id; }, this),
                    MemberCountDescription: this.model.get('MemberCount') === 1 ? 'Member' : 'Members',
                    SightingCountDescription: this.model.get('SightingCount') === 1 ? 'Sighting' : 'Sightings',
                    PostCountDescription: this.model.get('PostCount') === 1 ? 'Post' : 'Posts',
                    OrganisationAdministrators: this.organisationAdministrators,
                    ActivityTimeseries: this.activityTimeseries
                }
            };
        },

        onShow: function () {
            this._showDetails();
            return this;
        },

        showBootstrappedDetails: function () {
            this._showDetails();
        },

        _showDetails: function () {
            this.renderActivityTimeseries();

            var resizeTimer;
            var refresh = this.refresh;
            $(window).resize(function () {
                clearTimeout(resizeTimer);
                resizeTimer = setTimeout(function () {
                    refresh();
                }, 100);
            });
        },

        refresh: function () {
            this.$el.find('#activity-chart #chart-area').empty();
            this.renderActivityTimeseries();
        },

        renderActivityTimeseries: function () {
            this.$el.find('#activity-chart table')
                .visualize({ type: 'line', lineWeight: 2, width: this.$el.find('#chart-area').width() + 'px', height: '180px', colors: ['#308AE3', '#be1e2d', '#ee8310', '#4cba10', '#8d10ee', '#666699', '#5a3b16', '#26a4ed', '#f45a90', '#92d5ea'] })
                .appendTo('#activity-chart #chart-area')
                .trigger('visualizeRefresh');
        }
    });

    return OrganisationAboutView;

}); 