/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// SightingIdentificationFormView
// ------------------------------

define(['jquery', 'underscore', 'backbone', 'app', 'ich', 'views/sightingdetailsview', 'views/identificationformview', 'views/sightingidentificationsubformview', 'sightingnotedescriptions', 'moment', 'datepicker', 'multiselect', 'jqueryui/dialog', 'tipsy', 'tagging'],
function ($, _, Backbone, app, ich, SightingDetailsView, IdentificationFormView, SightingIdentificationSubFormView, sightingNoteDescriptions, moment) {

    var SightingIdentificationFormView = Backbone.Marionette.Layout.extend({

        viewType: 'form',

        className: 'form single sighting-identification-form',

        template: 'SightingIdentificationForm',

        regions: {
            sightingSection: '.sighting',
            sightingIdentificationSection: '.sighting-identification-fieldset'
        },

        events: {
            'click #cancel': '_cancel',
            'click #save': '_save'
        },

        initialize: function (options) {
            this.categorySelectList = options.categorySelectList;
            this.categories = options.categories;
            this.sighting = options.sighting;
        },

        serializeData: function () {
            return {
                Model: {
                    Sighting: this.sighting.toJSON(),
                    SightingIdentification: this.model.toJSON()
                }
            };
        },

        onShow: function () {
            var sightingView = new SightingDetailsView({ model: this.sighting, template: 'SightingFullFullDetails' });
            this.sightingView = sightingView;
            this.sightingSection.show(sightingView);

            var sightingIdentificationSubFormView = new SightingIdentificationSubFormView({ el: this.$el.find('.sighting-identification-fieldset'), model: this.model, categorySelectList: this.categorySelectList, categories: this.categories });
            this.sightingIdentificationSection.attachView(sightingIdentificationSubFormView);
            sightingIdentificationSubFormView.showBootstrappedDetails();

            this._showDetails();
        },

        //        onRender: function () {
        //            var sightingView = new SightingDetailsView({ model: new Observation(this.sighting) });
        //            this.sightingSection.show(sightingView);

        //            this._showDetails();
        //        },

        showBootstrappedDetails: function () {
            this.initializeRegions();

            var sightingView = new SightingDetailsView({ el: this.$el.find('.observation-details'), model: this.sighting, template: 'SightingFullFullDetails' });
            this.sightingView = sightingView;
            this.sightingSection.attachView(sightingView);
            sightingView.showBootstrappedDetails();

            var sightingIdentificationSubFormView = new SightingIdentificationSubFormView({ el: this.$el.find('.sighting-identification-fieldset'), model: this.model, categorySelectList: this.categorySelectList, categories: this.categories });
            this.sightingNoteSection.attachView(sightingIdentificationSubFormView);
            sightingIdentificationSubFormView.showBootstrappedDetails();

            this._showDetails();
        },

        _showDetails: function () {
            app.vent.on('view:render:complete', function () {
                this.sightingView.refresh();
            }, this);
        },

        _cancel: function () {
            app.showPreviousContentView();
        },

        _save: function () {
            this.model.save();
            app.showPreviousContentView();
        }
    });

    return SightingIdentificationFormView;

});