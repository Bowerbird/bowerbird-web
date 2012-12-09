/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// SightingNoteFormView
// --------------------

define(['jquery', 'underscore', 'backbone', 'app', 'ich', 'views/sightingdetailsview', 'views/identificationformview', 'views/sightingnotesubformview', 'sightingnotedescriptions', 'moment', 'datepicker', 'multiselect', 'jqueryui/dialog', 'tipsy', 'tagging'],
function ($, _, Backbone, app, ich, SightingDetailsView, IdentificationFormView, SightingNoteSubFormView, sightingNoteDescriptions, moment) {

    var SightingNoteFormView = Backbone.Marionette.Layout.extend({

        viewType: 'form',

        className: 'form single sighting-note-form',

        template: 'SightingNoteForm',

        regions: {
            sightingSection: '.sighting',
            sightingNoteSection: '.sighting-note-fieldset'
        },

        events: {
            'click #cancel': '_cancel',
            'click #save': '_save'
        },

        initialize: function (options) {
            this.categorySelectList = options.categorySelectList;
            this.categories = options.categories;
            this.descriptionTypesSelectList = options.descriptionTypesSelectList;
            this.sighting = options.sighting;
        },

        serializeData: function () {
            return {
                Model: {
                    Sighting:  this.sighting.toJSON(),
                    SightingNote: this.model.toJSON()
                }  
            };
        },

        onShow: function () {
            var sightingView = new SightingDetailsView({ model: this.sighting, template: 'SightingFullDetails' });
            this.sightingView = sightingView;
            this.sightingSection.show(sightingView);

            var sightingNoteSubFormView = new SightingNoteSubFormView({ el: this.$el.find('.sighting-note-fieldset'), model: this.model });
            this.sightingNoteSection.attachView(sightingNoteSubFormView);
            sightingNoteSubFormView.showBootstrappedDetails();
            //            var sightingNoteSubFormView = new SightingNoteSubFormView({ model: this.model });
            //            this.sightingNoteSection.show(sightingNoteSubFormView);

            this._showDetails();
        },

        //        onRender: function () {
        //            var sightingView = new SightingDetailsView({ model: new Observation(this.sighting) });
        //            this.sightingSection.show(sightingView);

        //            this._showDetails();
        //        },

        showBootstrappedDetails: function () {
            this.initializeRegions();

            var sightingView = new SightingDetailsView({ el: this.$el.find('.observation-details'), model: this.sighting, template: 'SightingFullDetails' });
            this.sightingView = sightingView;
            this.sightingSection.attachView(sightingView);
            sightingView.showBootstrappedDetails();

            var sightingNoteSubFormView = new SightingNoteSubFormView({ el: this.$el.find('.sighting-note-fieldset'), model: this.model });
            this.sightingNoteSection.attachView(sightingNoteSubFormView);
            sightingNoteSubFormView.showBootstrappedDetails();

            this._showDetails();
        },

        _showDetails: function () {
            app.vent.on('view:render:complete', function () {
                this.sightingView.refresh();
            }, this);
            // TODO: FIlter out already added description fields from list button
        },

        _cancel: function () {
            app.showPreviousContentView();
        },

        _save: function () {
            this.model.save();
            app.showPreviousContentView();
        }
    });

    return SightingNoteFormView;

});