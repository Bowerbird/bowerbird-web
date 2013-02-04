/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ObservationFormView
// -------------------

define(['jquery', 'underscore', 'backbone', 'app', 'ich', 'models/sightingnote', 'models/identification', 'views/locationformview', 'views/observationmediaformview', 'views/identificationformview', 'views/sightingidentificationsubformview', 'views/sightingnotesubformview', 'views/geospatialformview', 'moment', 'datepicker', 'multiselect', 'jqueryui/dialog'],
function ($, _, Backbone, app, ich, SightingNote, Identification, LocationFormView, ObservationMediaFormView, IdentificationFormView, IdentificationSubFormView, SightingNoteSubFormView, GeospatialFormView, moment) {

    var ObservationFormView = Backbone.Marionette.Layout.extend({

        viewType: 'form',

        className: 'form single observation-form',

        template: 'ObservationForm',

        regions: {
            media: '#media-resources-fieldset',
            location: '#location-fieldset',
            sightingNoteRegion: '.sighting-note-fieldset',
            identificationRegion: '.sighting-identification-fieldset'
        },

        events: {
            'click #cancel': '_cancel',
            'click #save': '_save',
            'change input#Title': '_contentChanged',
            'change input#ObservedOn': '_observedOnChanged',
            'change input#Address': '_contentChanged',
            //'change input#Latitude': '_latLongChanged',
            //'change input#Longitude': '_latLongChanged',
            'change input#AnonymiseLocation': '_anonymiseLocationChanged',
            'change #projects-field input:checkbox': '_projectsChanged',
            'change #category-field input:checkbox': '_categoryChanged',
            'click #location-options-button': '_locationOptionsClicked',
            'click #add-sighting-identification-button': '_showIdentificationForm',
            'click #add-sighting-note-button': '_showSightingNoteForm',
            'click .show-geospatial-form-button': '_showGeospatialForm'
        },

        observedOnUpdated: false, // When we derive the very first media, we extract the date and update the ObservedOn field. No further updates are allowed.

        initialize: function (options) {
            _.bindAll(this, '_showIdentificationForm');
            this.categorySelectList = options.categorySelectList;
            this.projectsSelectList = options.projectsSelectList;
            this.categories = options.categories;

            if (this.model.id) {
                this.observedOnUpdated = true;
                this.viewEditMode = 'update';
            } else {
                this.viewEditMode = 'create';
            }

            this.model.media.on('add', this.onMediaChanged, this);
        },

        serializeData: function () {
            return {
                Model: {
                    Observation: this.model.toJSON(),
                    CategorySelectList: this.categorySelectList,
                    ProjectsSelectList: this.projectsSelectList,
                    Create: this.viewEditMode === 'create',
                    Update: this.viewEditMode === 'update'
                }
            };
        },

        onMediaChanged: function (media) {
            if (!this.observedOnUpdated && media.mediaResource.get('Metadata').Created) {
                var created = moment(media.mediaResource.get('Metadata').Created);
                this.observedOnUpdated = true;
                this.model.set('ObservedOn', media.mediaResource.get('Metadata').Created);
                this.$el.find('#ObservedOn').val(created.format('D MMM YYYY'));
            }
        },

        onShow: function () {
            this._showDetails();
        },

        showBootstrappedDetails: function () {
            this.initializeRegions();
            this._showDetails();
        },

        _showDetails: function () {
            var locationFormView = new LocationFormView({ el: '#location-details', model: this.model });
            this.location.attachView(locationFormView);
            locationFormView.render();

            var observationMediaFormView = new ObservationMediaFormView({ el: '#media-details', model: this.model, collection: this.model.media });
            this.media.attachView(observationMediaFormView);
            observationMediaFormView.render();

            this.$el.find('#ObservedOn').val(moment(this.model.get('ObservedOn')).format('D MMMM YYYY'));
            this.observedOnDatePicker = this.$el.find('#ObservedOn').datepicker();

            this.categoryListSelectView = this.$el.find('#Category').multiSelect({
                selectAll: false,
                listHeight: 260,
                singleSelect: true,
                noOptionsText: 'No Categories',
                noneSelected: '<span class="default-option">Select Category</span>',
                oneOrMoreSelected: function (selectedOptions) {
                    var $selectedHtml = $('<span />');
                    _.each(selectedOptions, function (option) {
                        $selectedHtml.append('<span>' + option.text + '</span> ');
                    });
                    return $selectedHtml.children();
                }
            });

            this.projectListSelectView = this.$el.find('#Projects').multiSelect({
                selectAll: false,
                listHeight: 260,
                messageText: 'Select the projects to add this sighting to',
                noOptionsText: 'No Projects',
                noneSelected: '<span class="default-option">Select Projects</span>',
                renderOption: function (id, option) {
                    var html = '<label><input style="display:none;" type="checkbox" name="' + id + '[]" value="' + option.value + '"';
                    if (option.selected) {
                        html += ' checked="checked"';
                    }
                    var project = app.authenticatedUser.projects.get(option.value);

                    html += ' /><img src="' + project.get('Avatar').Image.Square100.Uri + '" alt="" />' + project.get('Name') + '</label>';
                    return html;
                },
                oneOrMoreSelected: function (selectedOptions) {
                    var $selectedHtml = $('<div />');
                    _.each(selectedOptions, function (option) {
                        var project = app.authenticatedUser.projects.get(option.value);
                        $selectedHtml.append('<span class="selected-project"><img src="' + project.get('Avatar').Image.Square100.Uri + '" alt="" /></span> ');
                    });
                    return $selectedHtml.children();
                }
            });

            this.$el.find('#Title, #Address').tipsy({ trigger: 'focus', gravity: 'w', fade: true, live: true });
            this.$el.find('#Category, #Projects').tipsy({ trigger: 'focus', gravity: 'e', fade: true, live: true });
        },

        _contentChanged: function (e) {
            var target = $(e.currentTarget);
            var data = {};
            data[target.attr('id')] = target.attr('value');
            this.model.set(data);

            if (target.attr('id') === 'Address') {
                this._latLongChanged(e);
            }
        },

        _observedOnChanged: function (e) {
            this.observedOnUpdated = true;
            var target = $(e.currentTarget);
            var data = {};
            data[target.attr('id')] = target.attr('value');
            this.model.set(data);
        },

        _latLongChanged: function (e) {
            //var oldPosition = { latitude: this.model.get('Latitude'), longitude: this.model.get('Longitude') };
            //var newPosition = { latitude: this.$el.find('#Latitude').val(), longitude: this.$el.find('#Longitude').val() };

            // log('lat/long changed', oldPosition, newPosition);

            //this.model.set('Latitude', newPosition.latitude);
            //this.model.set('Longitude', newPosition.longitude);

            // Only update pin if the location is different to avoid infinite loop
            //if (newPosition.latitude !== null && newPosition.longitude !== null && newPosition.latitude.trim() !== '' && newPosition.longitude.trim() !== '' && (oldPosition.latitude !== newPosition.latitude || oldPosition.longitude !== newPosition.longitude)) {
                //this.location.currentView.changeMarkerPosition(this.model.get('Latitude'), this.model.get('Longitude'), true);
            //}
        },

        _anonymiseLocationChanged: function (e) {
            var $checkbox = $(e.currentTarget);
            this.model.set({ AnonymiseLocation: $checkbox.attr('checked') == 'checked' ? true : false });
        },

        _projectsChanged: function (e) {
            var $checkbox = $(e.currentTarget);
            if ($checkbox.attr('checked') === 'checked') {
                var projectId = $checkbox.attr('value');
                this.model.addProject(projectId);
            } else {
                this.model.removeProject($checkbox.attr('value'));
            }
        },

        _categoryChanged: function (e) {
            var category = '';
            var $checkbox = $(e.currentTarget);
            if ($checkbox.attr('checked') === 'checked') {
                category = $checkbox.attr('value');
            }
            this.model.set('Category', category);
        },

        _locationOptionsClicked: function (e) {
            this.$el.find('#location-options').toggle();
        },

        _showIdentificationForm: function () {
            if (!this.identification) {
                this.identification = new Identification();
            }
            var identificationSubFormView = new IdentificationSubFormView({ model: this.identification, categories: this.categories, categorySelectList: this.categorySelectList });
            this.identificationSubFormView = identificationSubFormView;
            this.identificationRegion.show(identificationSubFormView);
        },

        _showSightingNoteForm: function () {
            if (!this.sightingNote) {
                this.sightingNote = new SightingNote();
            }
            var sightingNoteSubFormView = new SightingNoteSubFormView({ model: this.sightingNote, categories: this.categories, categorySelectList: this.categorySelectList });
            this.sightingNoteSubFormView = sightingNoteSubFormView;
            this.sightingNoteRegion.show(sightingNoteSubFormView);
        },

        _showGeospatialForm: function (e) {
            e.preventDefault();
            $('body').append('<div id="modal-dialog"></div>');

            var geospatialFormView = new GeospatialFormView({ el: $('#modal-dialog'), model: this.model });
            this.geospatialFormView = geospatialFormView;
            geospatialFormView.on('coords-done', this.onGeospatialDone, this);
            geospatialFormView.render();
        },

        onGeospatialDone: function (obs) {
            this.location.currentView.changeMarkerPosition(this.model.get('Latitude'), this.model.get('Longitude'), true);
        },

        _cancel: function () {
            app.showPreviousContentView();
        },

        _save: function () {
            if (this.media.currentView.progressCount > 0) {
                return;
            }

            //            if (this.viewEditMode == 'update') {
            //                this.model.set('Id', this.model.id.replace('observations/', ''));
            //            }

            if (this.identification) {
                this.model.setIdentification(this.identification);
            }

            if (this.sightingNote) {
                this.model.setSightingNote(this.sightingNote);
            }

            this.model.save();
            app.showPreviousContentView();
        }
    });

    return ObservationFormView;

});