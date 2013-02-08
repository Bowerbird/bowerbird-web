/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// AccountUpdateFormView
// ---------------------

define(['jquery', 'underscore', 'backbone', 'app', 'ich', 'loadimage', 'views/editavatarview', 'licences', 'fileupload', 'multiselect'],
function ($, _, Backbone, app, ich, loadImage, EditAvatarView, licences) {
    var AccountUpdateFormView = Backbone.Marionette.Layout.extend({

        viewType: 'form',

        className: 'form form-medium single user-form',

        template: 'AccountUpdateForm',

        regions: {
            avatar: '#avatar-fieldset'
        },

        events: {
            'click #cancel': '_cancel',
            'click #save': '_save',
            'change input#Name': '_contentChanged',
            'change input#Email': '_contentChanged',
            'change #Description': '_contentChanged',
            'change #timezone-field input:checkbox': '_timezoneChanged',
            'change #licence-field input:checkbox': '_defaultLicenceChanged'
        },

        initialize: function (options) {
            this.timezoneSelectList = options.timezoneSelectList;
            this.licenceSelectList = options.licenceSelectList;
        },

        serializeData: function () {
            return {
                Model: {
                    User: this.model.toJSON(),
                    TimezoneSelectList: this.timezoneSelectList,
                    LicenceSelectList: this.licenceSelectList,
                    Licences: _.map(licences.getAll(), function (licence) {
                        return {
                            Text: licence.Name,
                            Value: licence.Id,
                            Selected: licence.Id === this.model.get('DefaultLicence')
                        };
                    },
                    this)
                }
            };
        },

        onShow: function () {
            this._showDetails();
        },

        showBootstrappedDetails: function () {
            this.initializeRegions();
            this._showDetails();
        },

        _showDetails: function () {
            var editAvatarView = new EditAvatarView({ el: '.avatar-field', model: this.model });
            editAvatarView.render();

            this.timezoneListSelectView = this.$el.find('#Timezone').multiSelect({
                selectAll: false,
                listHeight: 260,
                singleSelect: true,
                noOptionsText: 'No Timezones',
                noneSelected: '<span class="default-option">Select Timezone</span>',
                oneOrMoreSelected: function (selectedOptions) {
                    var $selectedHtml = $('<span />');
                    _.each(selectedOptions, function (option) {
                        $selectedHtml.append('<span>' + option.text + '</span> ');
                    });
                    return $selectedHtml.children();
                }
            });

            var that = this;

            var defaultLicence = licences.get(' ');
            this.defaultLicenceListSelectView = this.$el.find("#DefaultLicence").multiSelect({
                selectAll: false,
                listHeight: 263,
                singleSelect: true,
                noOptionsText: 'No Licences',
                noneSelected: '<span class="default-option">Select Default Licence</span>',
                renderOption: function (id, option) {
                    var model = {
                        Model: {
                            Licence: licences.get(option.value),
                            Selected: option.selected
                        }
                    };

                    return $('<div />').append(ich.LicenceItem(model)).html();
                },
                oneOrMoreSelected: function (selectedOptions) {
                    var $selectedHtml = $('<span />');
                    _.each(selectedOptions, function (option) {
                        var licence = licences.get(option.value);
                        $selectedHtml.append('<span>' + licence.Name + '</span> ');
                        for (var x = 0; x < licence.Icons.length; x++) {
                            $selectedHtml.append('<img src="' + licence.Icons[x] + '" alt="" />');
                        }
                        that.selectedLicence = licence.Id;
                    });
                    return $selectedHtml.children();
                }
            });
        },

        _contentChanged: function (e) {
            //log('userFormLayoutView:_contentChanged');
            var target = $(e.currentTarget);
            var data = {};
            data[target.attr('id')] = target.attr('value');
            this.model.set(data);
        },

        _timezoneChanged: function (e) {
            var timezone = '';
            var $checkbox = $(e.currentTarget);
            if ($checkbox.attr('checked') === 'checked') {
                timezone = $checkbox.attr('value');
            }
            this.model.set('Timezone', timezone);
        },

        _defaultLicenceChanged: function (e) {
            var defaultLicence = '';
            var $checkbox = $(e.currentTarget);
            if ($checkbox.attr('checked') === 'checked') {
                defaultLicence = $checkbox.attr('value');
            }
            this.model.set('DefaultLicence', defaultLicence);
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

            this.$el.find('#Name, #Email, #Timezone, #DefaultLicence').removeClass('input-validation-error');

            if (_.any(errors, function (item) { return item.Field === 'Name'; })) {
                this.$el.find('#Name').addClass('input-validation-error');
            }
            if (_.any(errors, function (item) { return item.Field === 'Email' || item.Field === 'EmailInvalid'; })) {
                this.$el.find('#Email').addClass('input-validation-error');
            }
            if (_.any(errors, function (item) { return item.Field === 'Timezone'; })) {
                this.$el.find('#Timezone').addClass('input-validation-error');
            }
            if (_.any(errors, function (item) { return item.Field === 'DefaultLicence'; })) {
                this.$el.find('#DefaultLicence').addClass('input-validation-error');
            }
        },

        _cancel: function (e) {
            app.showPreviousContentView();
        },

        _save: function (e) {
            e.preventDefault();

            this.$el.find('#save').attr('disabled', 'disabled').val('Saving...');

            var that = this;

            this.model.save(null, {
                success: function (model, response, options) {
                    that.$el.find('#save').attr('disabled', 'disabled').val('Saved');
                    that.onValidation(that.model, []);
                    window.location.replace('/');
                },
                error: function (model, xhr, options) {
                    that.$el.find('#save').removeAttr('disabled').val('Save');

                    var data = JSON.parse(xhr.responseText);
                    that.onValidation(that.model, data.Model.Errors);
                    $('html, body').animate({ scrollTop: 0 }, 'slow');
                }
            });

            return false;
        }
    });

    return AccountUpdateFormView;
});