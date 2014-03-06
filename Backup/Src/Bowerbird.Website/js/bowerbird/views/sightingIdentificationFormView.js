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

            this.$el.find('#Identification').removeClass('input-validation-error');

            if (_.any(errors, function (item) { return item.Field === 'IsCustomIdentification'; })) {
                this.$el.find('#Identification').addClass('input-validation-error');
            }
        },

        _cancel: function () {
            app.showPreviousContentView();
        },

        _save: function () {
            this.$el.find('#save').attr('disabled', 'disabled').val('Saving...');

            var that = this;

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

    return SightingIdentificationFormView;

});