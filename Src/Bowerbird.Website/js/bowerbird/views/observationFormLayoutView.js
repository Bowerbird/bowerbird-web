/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ObservationFormLayoutView
// -------------------------

define(['jquery', 'underscore', 'backbone', 'app', 'ich', 'views/editmapview', 'views/observationmediaformview', 'views/identificationformview', 'moment', 'datepicker', 'multiselect', 'jqueryui/dialog'],
function ($, _, Backbone, app, ich, EditMapView, ObservationMediaFormView, IdentificationFormView, moment) {

    var ObservationFormLayoutView = Backbone.Marionette.Layout.extend({

        className: 'form observation-form',

        template: 'ObservationForm',

        regions: {
            media: '#media-resources-fieldset',
            map: '#location-fieldset'
        },

        events: {
            'click #cancel': '_cancel',
            'click #save': '_save',
            'change input#Title': '_contentChanged',
            'change input#ObservedOn': '_observedOnChanged',
            'change input#Address': '_contentChanged',
            'change input#Latitude': '_latLongChanged',
            'change input#Longitude': '_latLongChanged',
            'change input#IsIdentificationRequired': '_isIdentificationRequiredChanged',
            'change input#AnonymiseLocation': '_anonymiseLocationChanged',
            'change #projects-field input:checkbox': '_projectsChanged',
            'change #category-field input:checkbox': '_categoryChanged',
            'click #location-options-button': '_locationOptionsClicked',
            'click #identify-observation-option': '_showIdentificationForm'
        },

        observedOnUpdated: false, // When we derive the very first media, we extract the date and update the ObservedOn field. No further updates are allowed.

        initialize: function (options) {
            _.bindAll(this, '_showIdentificationForm');
            this.categories = options.categories;
            this.model.media.on('add', this.onMediaChanged, this);
        },

        serializeData: function () {
            return {
                Model: {
                    Observation: this.model.toJSON(),
                    Categories: this.categories
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
            var editMapView = new EditMapView({ el: '#location-details', model: this.model });
            this.map.attachView(editMapView);
            editMapView.render();

            var observationMediaFormView = new ObservationMediaFormView({ el: '#media-details', model: this.model, collection: this.model.media });
            this.media.attachView(observationMediaFormView);
            observationMediaFormView.render();

            this.$el.find('#ObservedOn').val(moment(this.model.get('ObservedOn')).format('D MMMM YYYY'));
            this.observedOnDatePicker = this.$el.find('#ObservedOn').datepicker();

            this.categoryListSelectView = this.$el.find('#Category').multiSelect({
                selectAll: false,
                listHeight: 260,
                singleSelect: true,
                messageText: 'Select a category, or <a href="#" id="identify-observation-option">identify the observation now</a>',
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

            if (!ich.templates.ProjectSelectItem) {
                ich.addTemplate('ProjectSelectItem', '{{#Projects}}<option value="{{Id}}">{{Name}}</option>{{/Projects}}');
            }

            // Add project options
            this.$el.find('#Projects').append(ich.ProjectSelectItem({ Projects: app.authenticatedUser.projects.toJSON() }));

            this.projectListSelectView = this.$el.find('#Projects').multiSelect({
                selectAll: false,
                listHeight: 260,
                messageText: 'You can select more than one project',
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
            $('body').append('<div id="modal-dialog"></div>');
            var identificationFormView = new IdentificationFormView({ el: $('#modal-dialog') });
            identificationFormView.on('identificationdone', this._onIdentificationDone, this);

            $('#Category').multiSelectOptionsHide();

            identificationFormView.render();

            e.stopPropagation();
        },

        _observedOnChanged: function (e) {
            this.observedOnUpdated = true;
        },

        _latLongChanged: function (e) {
            var oldPosition = { latitude: this.model.get('Latitude'), longitude: this.model.get('Longitude') };
            var newPosition = { latitude: this.$el.find('#Latitude').val(), longitude: this.$el.find('#Longitude').val() };

            this.model.set('Latitude', newPosition.latitude);
            this.model.set('Longitude', newPosition.longitude);

            // Only update pin if the location is different to avoid infinite loop
            if (newPosition.Latitude != null && newPosition.Longitude != null && (oldPosition.Latitude !== newPosition.Latitude || oldPosition.Longitude !== newPosition.Longitude)) {
                this.editMapView.changeMarkerPosition(this.model.get('Latitude'), this.model.get('Longitude'));
            }
        },

        _isIdentificationRequiredChanged: function (e) {
            var $checkbox = $(e.currentTarget);
            this.model.set({ IsIdentificationRequired: $checkbox.attr('checked') == 'checked' ? true : false });
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
            var $checkbox = $(e.currentTarget);
            if ($checkbox.attr('checked') === 'checked') {
                this.model.set('Category', $checkbox.attr('value'));
            } else {
                this.model.set('Category', '');
            }
        },

        _locationOptionsClicked: function (e) {
            this.$el.find('#location-options').toggle();
        },

        _cancel: function () {
            app.showPreviousContentView();
        },

        _save: function () {
            if (this.media.currentView.progressCount > 0) {
                return;
            }

            this.model.save();
            app.showPreviousContentView();
        }
    });

    return ObservationFormLayoutView;

});