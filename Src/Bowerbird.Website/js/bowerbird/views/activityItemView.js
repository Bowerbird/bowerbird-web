/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/icanhaz/icanhaz.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ActivityItemView
// ----------------

// Shows an individual stream item
define(['jquery', 'underscore', 'backbone', 'ich', 'app', 'models/sighting', 'models/sightingnote', 'models/identification', 'models/post', 'views/sightingdetailsview', 'views/sightingnotedetailsview', 'views/identificationdetailsview', 'views/postdetailsview', 'moment', 'timeago', 'tipsy'],
function ($, _, Backbone, ich, app, Sighting, SightingNote, Identification, Post, SightingDetailsView, SightingNoteDetailsView, IdentificationDetailsView, PostDetailsView, moment) {

    var ActivityItemView = Backbone.Marionette.Layout.extend({
        tagName: 'li',

        className: 'activity-item',

        template: 'ActivityItem',

        events: {
            'click .view-button': 'showItem',
            'click .edit-observation-button': 'showItem'
        },

        regions: {
            details: '.details',
            summarySighting: '.sighting-summary-details'
        },

        initialize: function (options) {
            _.bindAll(this, 'refresh');
        },

        onRender: function () {
            var detailsView = null;
            var sightingSummaryView = null;

            if (this.model.get('Type') === "sightingadded") {
                detailsView = new SightingDetailsView({ className: 'observation-details', template: 'SightingFullDetails', model: new Sighting(this.model.get('ObservationAdded').Observation), isObservationActviityItem: true });
            }

            if (this.model.get('Type') === "sightingnoteadded") {
                sightingSummaryView = new SightingDetailsView({ template: 'SightingSummaryDetails', model: new Sighting(this.model.get('SightingNoteAdded').Sighting) });
                detailsView = new SightingNoteDetailsView({ model: new SightingNote(this.model.get('SightingNoteAdded').SightingNote), sighting: new Sighting(this.model.get('SightingNoteAdded').Sighting) });
            }

            if (this.model.get('Type') === "identificationadded") {
                sightingSummaryView = new SightingDetailsView({ template: 'SightingSummaryDetails', model: new Sighting(this.model.get('IdentificationAdded').Sighting) });
                detailsView = new IdentificationDetailsView({ model: new Identification(this.model.get('IdentificationAdded').Identification), sighting: new Sighting(this.model.get('IdentificationAdded').Sighting) });
            }

            if (this.model.get('Type') === "postadded") {
                detailsView = new PostDetailsView({ model: new Post(this.model.get('PostAdded').Post), template: 'PostFullDetails' });
            }

            this.details.show(detailsView);

            if (sightingSummaryView) {
                this.summarySighting.show(sightingSummaryView);
            }

            this._showDetails();
        },

        showBootstrappedDetails: function () {
            this.initializeRegions();

            var detailsView = null;
            var sightingSummaryView = null;

            if (this.model.get('Type') === "sightingadded") {
                detailsView = new SightingDetailsView({ className: 'observation-details', template: 'SightingFullDetails', el: this.$el.find('.observation-details'), model: new Sighting(this.model.get('ObservationAdded').Observation) });
            }
            if (this.model.get('Type') === "sightingnoteadded") {
                sightingSummaryView = new SightingDetailsView({ el: this.$el.find('.sighting-summary-details'), template: 'SightingSummaryDetails', model: new Sighting(this.model.get('SightingNoteAdded').Sighting) });
                detailsView = new SightingNoteDetailsView({ el: this.$el.find('.sighting-note-details'), model: new SightingNote(this.model.get('SightingNoteAdded').SightingNote), sighting: new Sighting(this.model.get('SightingNoteAdded').Sighting) });
            }
            if (this.model.get('Type') === "identificationadded") {
                sightingSummaryView = new SightingDetailsView({ el: this.$el.find('.sighting-summary-details'), template: 'SightingSummaryDetails', model: new Sighting(this.model.get('IdentificationAdded').Sighting) });
                detailsView = new IdentificationDetailsView({ el: this.$el.find('.identification-details'), model: new Identification(this.model.get('IdentificationAdded').Identification), sighting: new Sighting(this.model.get('IdentificationAdded').Sighting) });
            }
            if (this.model.get('Type') === "postadded") {
                detailsView = new PostDetailsView({ el: this.$el.find('.post-details'), model: new Post(this.model.get('PostAdded').Post) });
            }

            this.details.attachView(detailsView);
            detailsView.showBootstrappedDetails();

            if (sightingSummaryView) {
                this.summarySighting.attachView(sightingSummaryView);
                sightingSummaryView.showBootstrappedDetails();
            }

            this._showDetails();
        },

        _showDetails: function () {
            this.$el.find('.time-description').text(moment(this.model.get('CreatedDateTime')).format('D MMMM YYYY h:mma'));
            this.$el.find('.time-description').timeago();

            this.$el.find('.actions a').tipsy({ gravity: 's', html: true });

//            if (this.model.get('User').Id !== app.authenticatedUser.user.id) {
//                this.$el.find('.edit-observation-button').hide();
//            }
        },

        refresh: function () {
            if (this.details.currentView && this.details.currentView.refresh) {
                this.details.currentView.refresh();
            }
            if (this.summarySighting.currentView && this.summarySighting.currentView.refresh) {
                this.summarySighting.currentView.refresh();
            }
        },

        showItem: function (e) {
            e.preventDefault();
            this.$el.find('.actions a').tipsy.revalidate();
            Backbone.history.navigate($(e.currentTarget).attr('href'), { trigger: true });
        }
    });

    return ActivityItemView;
});