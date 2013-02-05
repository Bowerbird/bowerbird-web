/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// SightingIdentificationFormView
// ------------------------------

define(['jquery', 'underscore', 'backbone', 'app', 'ich', 'views/sightingdetailsview', 'views/identificationformview', 'views/sightingidentificationsubformview', 'moment', 'datepicker', 'multiselect', 'jqueryui/dialog', 'tipsy', 'tagging'],
function ($, _, Backbone, app, ich, SightingDetailsView, IdentificationFormView, SightingIdentificationSubFormView, moment) {

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

            this.model.on('validated', this.onValidation, this);
        },

        serializeData: function () {
            return {
                Model: {
                    Sighting: this.sighting.toJSON(),
                    Identification: this.model.toJSON()
                }
            };
        },

        onShow: function () {
            var sightingView = new SightingDetailsView({ model: this.sighting, className: 'observation-details', template: 'SightingFullDetails' });
            this.sightingView = sightingView;
            this.sightingSection.show(sightingView);

            var sightingIdentificationSubFormView = new SightingIdentificationSubFormView({ el: this.$el.find('.sighting-identification-fieldset'), model: this.model, categorySelectList: this.categorySelectList, categories: this.categories });
            this.sightingIdentificationSection.attachView(sightingIdentificationSubFormView);
            sightingIdentificationSubFormView.showBootstrappedDetails();

            this._showDetails();
        },

        showBootstrappedDetails: function () {
            this.initializeRegions();

            var sightingView = new SightingDetailsView({ el: this.$el.find('.observation-details'), model: this.sighting, template: 'SightingFullDetails' });
            this.sightingView = sightingView;
            this.sightingSection.attachView(sightingView);
            sightingView.showBootstrappedDetails();

            var sightingIdentificationSubFormView = new SightingIdentificationSubFormView({ el: this.$el.find('.sighting-identification-fieldset'), model: this.model, categorySelectList: this.categorySelectList, categories: this.categories });
            this.sightingIdentificationSection.attachView(sightingIdentificationSubFormView);
            sightingIdentificationSubFormView.showBootstrappedDetails();

            this._showDetails();
        },

        _showDetails: function () {
            app.vent.on('view:render:complete', function () {
                this.sightingView.refresh();
            }, this);
        },

        onValidation: function (obs, errors) {
            if (errors.length == 0) {
                this.$el.find('.validation-summary').slideUp(function () { $(this).remove(); });
            }

            if (errors.length > 0) {
                if (this.$el.find('.validation-summary').length == 0) {
                    this.$el.find('form').prepend(ich.ValidationSummary({
                        SummaryMessage: 'Please correct the following before continuing:',
                        Errors: errors
                    }));
                    this.$el.find('.validation-summary').slideDown();
                } else {
                    var that = this;
                    // Remove items
                    this.$el.find('.validation-summary li').each(function () {
                        var $li = that.$el.find(this);
                        var found = _.find(errors, function (err) {
                            return 'validation-field-' + err.Field === $li.attr('class');
                        });
                        if (!found) {
                            $li.slideUp(function () { $(this).remove(); });
                        }
                    });

                    // Add items
                    _.each(errors, function (err) {
                        if (this.$el.find('.validation-field-' + err.Field).length === 0) {
                            var li = $('<li class="validation-field-' + err.Field + '">' + err.Message + '</li>').css({ display: 'none' });
                            this.$el.find('.validation-summary ul').append(li);
                            li.slideDown();
                        }
                    }, this);
                }
            }

            //            if (errors.length == 0) {
            //                //this.$el.find('#save').removeAttr('disabled');
            //            } else {
            //                //this.$el.find('#save').attr('disabled', 'disabled');
            //            }

            this.$el.find('.form #selected-identification-field #Identification').removeClass('input-validation-error');

            if (_.any(errors, function (item) { return item.Field === 'Taxonomy'; })) {
                this.$el.find('.form #selected-identification-field #Identification').addClass('input-validation-error');
            }
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