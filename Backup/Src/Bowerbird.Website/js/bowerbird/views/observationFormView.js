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
            'change input#AnonymiseLocation': '_anonymiseLocationChanged',
            'change #projects-field input:checkbox': '_projectsChanged',
            'change #category-field input:checkbox': '_categoryChanged',
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
                    ProjectsSelectList: this.projectsSelectList
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

            observationMediaFormView.on('upload-error', this.onValidation, this);

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
        },

        _observedOnChanged: function (e) {
            this.observedOnUpdated = true;
            var target = $(e.currentTarget);
            var data = {};
            data[target.attr('id')] = target.attr('value');
            this.model.set(data);
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

        _showIdentificationForm: function () {
            if (!this.identification) {
                this.identification = new Identification();
                this.identification.on('validated', this.onIdentificationValidation, this);
            }
            var identificationSubFormView = new IdentificationSubFormView({ model: this.identification, categories: this.categories, categorySelectList: this.categorySelectList });
            this.identificationSubFormView = identificationSubFormView;
            this.identificationRegion.show(identificationSubFormView);
        },

        _showSightingNoteForm: function () {
            if (!this.sightingNote) {
                this.sightingNote = new SightingNote();
                this.sightingNote.on('validated', this.onNoteValidation, this);
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

        onValidation: function (obs, errors) {
            if (errors.length == 0) {
                this.$el.find('.validation-summary').slideUp(function () { $(this).remove(); });
            }

            if (errors.length > 0) {
                if (this.$el.find('.validation-summary').length == 0) {
                    this.$el.find('.form-details').prepend(ich.ValidationSummary({
                        SummaryMessage: 'Please correct the following before continuing:',
                        Errors: errors,
                        // Due to a bug in mustache.js where you can't reference a parent element in a string array loop, we have to build the HTML here
                        Error: function () {
                            return _.map(this.Messages, function (message) {
                                return '<li class="validation-field-' + this.Field + '">' + message + '</li>';
                            }, this).join('\n');
                        }
                    }));
                    this.$el.find('.validation-summary').slideDown();
                } else {
                    var that = this;
                    // Remove items that are now valid
                    this.$el.find('.validation-summary li').each(function () {
                        var $li = that.$el.find(this);
                        var found = _.find(errors, function (err) {
                            return _.find(err.Messages, function (message) {
                                return 'validation-field-' + err.Field === $li.attr('class') && message === $li.text();
                            });
                        });
                        if (!found) {
                            $li.slideUp(function () { $(this).remove(); });
                        }
                    });

                    // Add items
                    var lis = this.$el.find('.validation-summary li');
                    _.each(errors, function (err) {
                        _.each(err.Messages, function (message) {
                            // Only add if the class and text is not found in li list
                            var found = _.find(lis, function (li) {
                                var $li = $(li);
                                return $li.attr('class') === 'validation-field-' + err.Field && $li.text() === message;
                            });

                            if (!found) {
                                var linew = $('<li class="validation-field-' + err.Field + '">' + message + '</li>').css({ display: 'none' });
                                that.$el.find('.validation-summary ul').append(linew);
                                linew.slideDown();
                            }
                        });
                    }, this);
                }
            }

            this.$el.find('#Title, #Category, #pin-field, .observation-media-items, #location-coordinates').removeClass('input-validation-error');
            if (this.identification) {
                this.identificationRegion.currentView.$el.find('#selected-identification-field #Identification').removeClass('input-validation-error');
            }
            if (this.sightingNote) {
                this.sightingNoteRegion.currentView.$el.find('textarea.note-description, .tagit').removeClass('input-validation-error');
            }
            
            // Observation
            if (_.any(errors, function (item) { return item.Field === 'Title'; })) {
                this.$el.find('#Title').addClass('input-validation-error');
            }
            if (_.any(errors, function (item) { return item.Field === 'Category'; })) {
                this.$el.find('#Category').addClass('input-validation-error');
            }
            if (_.any(errors, function (item) { return item.Field === 'Location'; })) {
                this.$el.find('#pin-field').addClass('input-validation-error');
            }
            if (_.any(errors, function (item) { return item.Field === 'Media'; })) {
                this.$el.find('.observation-media-items').addClass('input-validation-error');
            }
            if (_.any(errors, function (item) { return item.Field === 'Latitude'; })) {
                this.$el.find('#location-coordinates').addClass('input-validation-error');
            }

            // Identification
            if (_.any(errors, function (item) { return item.Field === 'IsCustomIdentification'; })) {
                this.identificationRegion.currentView.$el.find('#selected-identification-field #Identification').addClass('input-validation-error');
            }

            // Note           
            if (_.any(errors, function (item) { return item.Field === 'Descriptions'; })) {
                this.sightingNoteRegion.currentView.$el.find('textarea.note-description, .tagit').addClass('input-validation-error');
            }
        },

        _cancel: function () {
            app.showPreviousContentView();
        },

        _save: function () {
            if (this.media.currentView.progressCount > 0) {
                return;
            }

            this.$el.find('#save').attr('disabled', 'disabled').val('Saving...');

            var that = this;

            if (this.identification) {
                this.model.setIdentification(this.identification);
            }

            if (this.sightingNote) {
                this.model.setSightingNote(this.sightingNote);
            }

            this.model.save(null, {
                success: function (model, response, options) {
                    that.$el.find('#save').attr('disabled', 'disabled').val('Saved');
                    that.onValidation(that.model, []);
                    app.showPreviousContentView();
                },
                error: function (model, xhr, options) {
                    that.$el.find('#save').removeAttr('disabled').val('Save');

                    var data = JSON.parse(xhr.responseText);
                    that.onValidation(that.model, data.Model.Errors);
                    $('html, body').animate({ scrollTop: 0 }, 'slow');
                }
            });
        }
    });

    return ObservationFormView;

});