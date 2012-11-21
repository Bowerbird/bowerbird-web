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
define(['jquery', 'underscore', 'backbone', 'ich', 'app', 'models/sighting', 'models/sightingnote', 'views/sightingdetailsview', 'views/sightingnotedetailsview', 'moment', 'timeago', 'tipsy'],
function ($, _, Backbone, ich, app, Sighting, SightingNote, SightingDetailsView, SightingNoteDetailsView, moment) {

    var ActivityItemView = Backbone.Marionette.Layout.extend({
        tagName: 'li',

        className: 'activity-item',

        template: 'ActivityItem',
        
        events: {
            'click .add-note-button': 'showNoteForm',
            'click .view-button': 'showItem',
            'click .edit-observation-button': 'showItem'
        },

        regions: {
            details: '.details' 
        },

        initialize: function (options) {
            _.bindAll(this, 'refresh');
        },

        onRender: function () {
            if (this.model.get('Type') === "sightingadded") {
                var detailsView = new SightingDetailsView({ model: new Sighting(this.model.get('ObservationAdded').Observation) });
                this.details.show(detailsView);
            }

            if (this.model.get('Type') === "sightingnoteadded") {
                var detailsView = new SightingNoteDetailsView({ model: new SightingNote(this.model.get('SightingNoteAdded').SightingNote), sighting: new Sighting(this.model.get('SightingNoteAdded').Sighting) });
                this.details.show(detailsView);
            }

            this._showDetails();
        },

        showBootstrappedDetails: function () {
            this.initializeRegions();
            var detailsView = null;
            if (this.model.get('Type') === "sightingadded") {
                var detailsView = new SightingDetailsView({ el: this.$el.find('.observation-details'), model: new Sighting(this.model.get('ObservationAdded').Observation) });
            }
            if (this.model.get('Type') === "sightingnoteadded") {
                var detailsView = new SightingNoteDetailsView({ el: this.$el.find('.sighting-note-details'), model: new SightingNote(this.model.get('SightingNoteAdded').SightingNote), sighting: new Sighting(this.model.get('SightingNoteAdded').Sighting) });
            }
            this.details.attachView(detailsView);
            detailsView.showBootstrappedDetails();
            
            this._showDetails();
        },

        _showDetails: function () {
            this.$el.find('.time-description').text(moment(this.model.get('CreatedDateTime')).format('D MMMM YYYY h:mma'));
            this.$el.find('.time-description').timeago();

//            if (this.model.get('Type') === 'sightingadded') {
//                this.$el.find('h3 a').on('click', function (e) {
//                    e.preventDefault();
//                    Backbone.history.navigate($(this).attr('href'), { trigger: true });
//                    return false;
//                });
//            }

            this.$el.find('.actions a').tipsy({ gravity: 's', html: true });
        },

        refresh: function () {
            if (this.details.currentView) {
                this.details.currentView.refresh();
            }
        },

        showNoteForm: function (e) {
            e.preventDefault();
            this.$el.find('.actions a').tipsy.revalidate();
            Backbone.history.navigate($(e.currentTarget).attr('href'), { trigger: true });
        },
        
        showItem: function (e) {
            e.preventDefault();
            this.$el.find('.actions a').tipsy.revalidate();
            Backbone.history.navigate($(e.currentTarget).attr('href'), { trigger: true });
        }
    });

    return ActivityItemView;
});