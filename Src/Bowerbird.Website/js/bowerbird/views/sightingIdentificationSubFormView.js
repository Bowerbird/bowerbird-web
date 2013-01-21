/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// SightingIdentificationSubFormView
// ---------------------------------

define(['jquery', 'underscore', 'backbone', 'app', 'ich', 'models/identification', 'views/sightingdetailsview', 'views/identificationformview', 'sightingnotedescriptions', 'moment', 'datepicker', 'multiselect', 'jqueryui/dialog', 'tipsy'],
function ($, _, Backbone, app, ich, Identification, SightingDetailsView, IdentificationFormView, sightingNoteDescriptions, moment) {

    var SightingIdentificationSubFormView = Backbone.Marionette.ItemView.extend({

        template: 'SightingIdentificationSubForm',

        events: {
            'click #add-identification-button': '_showIdentificationForm',
            'change #Comments': '_onCommentsChanged'
        },

        initialize: function (options) {
            _.bindAll(this, '_onIdentificationDone', '_showIdentificationForm', '_showIdentification');

            this.categorySelectList = options.categorySelectList;
            this.categories = options.categories;
            this.sighting = options.sighting;

            this.model.on('change:Taxonomy', this._showIdentification);
        },

        serializeData: function () {
            return {
                Model: {
                    SightingIdentification: this.model.toJSON()
                    //Sighting: this.sighting
                    //                    CategorySelectList: this.categorySelectList,
                    //                    ProjectsSelectList: this.projectsSelectList
                }
            };
        },

        onShow: function () {
            this._showDetails();
            return this;
        },

        //        onRender: function () {
        //            this._showDetails();
        //            return this;
        //        },

        showBootstrappedDetails: function () {
            this._showDetails();
        },

        _showDetails: function () {
        },

        _onCommentsChanged: function(e) {
            this.model.set('Comments', $(e.currentTarget).val());
        },

        _showIdentificationForm: function (e) {
            e.preventDefault();

            $('body').append('<div id="modal-dialog"></div>');
            var tempIdentification = new Identification(this.model.attributes);
            this.identificationFormView = new IdentificationFormView({ el: $('#modal-dialog'), categories: this.categories, categorySelectList: this.categorySelectList, model: tempIdentification });
            this.identificationFormView.on('identificationdone', this._onIdentificationDone, this);

            this.identificationFormView.render();
        },
        
        _onIdentificationDone: function (identification) {
            log('identification done', identification);

            this.model.set(identification.attributes);
        },

        _showIdentification: function (model) {
            log('identification changed', model);

            this.$el.find('#Identification').html(ich.Identification(model.toJSON()));
        }
    });

    return SightingIdentificationSubFormView;

});