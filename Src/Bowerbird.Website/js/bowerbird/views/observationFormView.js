/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ObservationFormView
// -------------------

define(['jquery', 'underscore', 'backbone', 'app', 'ich', 'models/sightingnote', 'models/identification', 'views/locationformview', 'views/observationmediaformview', 'views/identificationformview', 'views/sightingnotesubformview', 'moment', 'datepicker', 'multiselect', 'jqueryui/dialog'],
function ($, _, Backbone, app, ich, SightingNote, Identification, LocationFormView, ObservationMediaFormView, IdentificationFormView, SightingNoteSubFormView, moment) {

    var ObservationFormView = Backbone.Marionette.Layout.extend({

        viewType: 'form',

        className: 'form single observation-form',

        template: 'ObservationForm',

        regions: {
            media: '#media-resources-fieldset',
            location: '#location-fieldset',
            sightingNoteRegion: '.sighting-note-fieldset'
        },

        events: {
            'click #cancel': '_cancel',
            'click #save': '_save',
            'change input#Title': '_contentChanged',
            'change input#ObservedOn': '_observedOnChanged',
            'change input#Address': '_contentChanged',
            'change input#Latitude': '_latLongChanged',
            'change input#Longitude': '_latLongChanged',
            'change input#AnonymiseLocation': '_anonymiseLocationChanged',
            'change #projects-field input:checkbox': '_projectsChanged',
            'change #category-field input:checkbox': '_categoryChanged',
            'click #location-options-button': '_locationOptionsClicked',
            'click #identify-observation-option': '_showIdentificationForm',
            'click #add-sighting-note-button': '_showSightingNoteForm'
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
                messageText: 'Select a category, or <a href="#" id="identify-observation-option">identify the sighting</a>',
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

        _showIdentificationForm: function (e) {
            e.preventDefault();
            var that = this;

            // If sighting note exists, and id exists, send it to ID form;
            // Else if category exists, get id and send it to ID form;
            // Else nothing exists, send empty id to ID form.
            if (this.sightingNote && this.sightingNote.get('Taxonomy') != '') {
                $.ajax({
                    url: '/species?query=' + that.sightingNote.get('Taxonomy') + '&field=taxonomy'
                }).done(function (data) {
                    that._renderIdentificationForm(new Identification(data.Model.Species.PagedListItems[0]));
                });
            } else if (this.model.get('Category') !== '') {
                $.ajax({
                    url: '/species?query=' + _.find(that.categories, function (item) { return item.Name === this.model.get('Category'); }, that).Taxonomy + '&field=taxonomy'
                }).done(function (data) {
                    that._renderIdentificationForm(new Identification(data.Model.Species.PagedListItems[0]));
                });
            } else {
                this._renderIdentificationForm(new Identification());
            }
        },

        _renderIdentificationForm: function (identification) {
            $('body').append('<div id="modal-dialog"></div>');
            this.identificationFormView = new IdentificationFormView({ el: $('#modal-dialog'), categories: this.categories, categorySelectList: this.categorySelectList, model: identification });
            this.identificationFormView.on('identificationdone', this._onIdentificationDone, this);

            $('#Category').multiSelectOptionsHide();

            this.identificationFormView.render();
        },

        _onIdentificationDone: function (identification) {
            if (identification.get('Category') !== this.model.get('Category')) {
                this.$el.find('input[name="Category[]"][value="' + identification.get('Category') + '"]').click();
                this.$el.find('#Category').html('<span><span>' + identification.get('Category') + '</span></span><i></i>');
            }

            this._setCategory(identification.get('Category'));
            this._setIdentification(identification);
        },

        _observedOnChanged: function (e) {
            this.observedOnUpdated = true;
            var target = $(e.currentTarget);
            var data = {};
            data[target.attr('id')] = target.attr('value');
            this.model.set(data);
        },

        _latLongChanged: function (e) {
            var oldPosition = { latitude: this.model.get('Latitude'), longitude: this.model.get('Longitude') };
            var newPosition = { latitude: this.$el.find('#Latitude').val(), longitude: this.$el.find('#Longitude').val() };

            this.model.set('Latitude', newPosition.latitude);
            this.model.set('Longitude', newPosition.longitude);

            // Only update pin if the location is different to avoid infinite loop
            if (newPosition.Latitude != null && newPosition.Longitude != null && (oldPosition.Latitude !== newPosition.Latitude || oldPosition.Longitude !== newPosition.Longitude)) {
                this.locationFormView.changeMarkerPosition(this.model.get('Latitude'), this.model.get('Longitude'));
            }
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
            this._setCategory(category);
            this._setIdentification(null);
        },

        _setCategory: function (category) {
            this.model.set('Category', category);
        },

        _setIdentification: function (identification) {
            if (identification && !this.sightingNote) {
                this.sightingNote = new SightingNote();
                this._showSightingNoteForm();
            }
            if (this.sightingNote) {
                this.sightingNote.setIdentification(identification);
            }
        },

        _locationOptionsClicked: function (e) {
            this.$el.find('#location-options').toggle();
        },

        _showSightingNoteForm: function () {
            if (!this.sightingNote) {
                this.sightingNote = new SightingNote();
            }
            var sightingNoteSubFormView = new SightingNoteSubFormView({ model: this.sightingNote });
            this.sightingNoteSubFormView = sightingNoteSubFormView;
            this.sightingNoteRegion.show(sightingNoteSubFormView);
        },

        _cancel: function () {
            app.showPreviousContentView();
        },

        _save: function () {
            if (this.media.currentView.progressCount > 0) {
                return;
            }

            if (this.viewEditMode == 'update') {
                this.model.set('Id', this.model.id.replace('observations/', ''));
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