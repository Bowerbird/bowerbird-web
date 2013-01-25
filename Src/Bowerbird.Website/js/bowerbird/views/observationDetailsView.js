/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ObservationDetailsView
// ----------------------

define(['jquery', 'underscore', 'backbone', 'app', 'ich', 'views/sightingdetailsview', 'views/sightingnotedetailsview', 'views/identificationdetailsview', 'moment', 'datepicker', 'multiselect', 'jqueryui/dialog', 'tipsy', 'tagging'],
function ($, _, Backbone, app, ich, SightingDetailsView, SightingNoteDetailsView, IdentificationDetailsView, moment) {

    var ObservationDetailsView = Backbone.Marionette.Layout.extend({
        viewType: 'form',

        className: 'observation single',

        regions: {
            sightingSection: '.sighting',
            identificationsSection: '.identifications',
            sightingNotesSection: '.sighting-notes'
        },

        initialize: function (options) {
            if (!options.template) {
                this.template = 'ObservationIndex';
            } else {
                this.template = options.template;
            }
            this.identifications = options.identifications;
            this.sightingNotes = options.sightingNotes;
        },

        serializeData: function () {
            return {
                Model: {
                    Observation: this.model.toJSON()
                }
            };
        },

        onShow: function () {
            var sightingView = new SightingDetailsView({ el: this.$el.find('.observation-details'), className: 'observation-details', model: this.model, template: 'SightingFullFullDetails' });
            this.sightingView = sightingView;
            this.sightingSection.show(sightingView);

            var sightingNoteEls = this.$el.find('.sighting-note');
            this.sightingNotes.each(function (item, index) {
                var childView = new SightingNoteDetailsView({ el: $(sightingNoteEls[index]), model: item, tagName: 'li', sighting: this.model });
                childView.showBootstrappedDetails();
                childView.delegateEvents();
            }, this);

            var identificationEls = this.$el.find('.identification');
            this.identifications.each(function (item, index) {
                var childView = new IdentificationDetailsView({ el: $(identificationEls[index]), model: item, tagName: 'li', sighting: this.model });
                childView.showBootstrappedDetails();
                childView.delegateEvents();
            }, this);

            this._showDetails();
        },

        showBootstrappedDetails: function () {
            this.initializeRegions();

            var sightingView = new SightingDetailsView({ el: this.$el.find('.observation-details'), model: this.model, template: 'SightingFullFullDetails' });
            this.sightingView = sightingView;
            this.sightingSection.attachView(sightingView);
            sightingView.showBootstrappedDetails();

            var sightingNoteEls = this.$el.find('.sighting-note');
            this.sightingNotes.each(function (item, index) {
                var childView = new SightingNoteDetailsView({ el: $(sightingNoteEls[index]), model: item, tagName: 'li', sighting: this.model });
                childView.showBootstrappedDetails();
                childView.delegateEvents();
            }, this);

            var identificationEls = this.$el.find('.identification');
            this.identifications.each(function (item, index) {
                var childView = new IdentificationDetailsView({ el: $(identificationEls[index]), model: item, tagName: 'li', sighting: this.model });
                childView.showBootstrappedDetails();
                childView.delegateEvents();
            }, this);

            this._showDetails();
        },

        _showDetails: function () {
            app.vent.on('view:render:complete', function () {
                this.sightingView.refresh();
            }, this);
        }
    });

    return ObservationDetailsView;

});