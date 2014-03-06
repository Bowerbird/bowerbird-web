/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/jquery/jquery.fileupload.js" />
/// <reference path="../../libs/jquery/load-image.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ProjectFormView
// ---------------

define(['jquery', 'underscore', 'backbone', 'app', 'ich', 'loadimage', 'views/editavatarview', 'views/backgroundimageformview', 'fileupload', 'multiselect'],
function ($, _, Backbone, app, ich, loadImage, AvatarImageFormView, BackgroundImageFormView) {

    var ProjectFormView = Backbone.Marionette.Layout.extend({
        viewType: 'form',

        className: 'form form-medium single project-form',

        template: 'ProjectForm',

        regions: {
            //avatar: '#avatar-fieldset',
            backgroundRegion: '#background-fieldset'
        },

        events: {
            'click #cancel': '_cancel',
            'click #save': '_save',
            'change input#Name': '_contentChanged',
            'change textarea#Description': '_contentChanged',
            'change input#Website': '_contentChanged',
            'change #categories-field input:checkbox': '_categoriesChanged'
        },

        initialize: function (options) {
            if (this.model.id) {
                this.viewEditMode = 'update';
            } else {
                this.viewEditMode = 'create';
            }
            this.categoriesSelectList = options.categoriesSelectList;

            this.model.on('validated', this.onValidation, this);
        },

        serializeData: function () {
            return {
                Model: {
                    Project: this.model.toJSON(),
                    CategoriesSelectList: this.categoriesSelectList
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
            var avatarImageFormView = new AvatarImageFormView({ el: '.avatar-field', model: this.model });
            this.avatarImageFormView = avatarImageFormView;
            avatarImageFormView.render();

            var backgroundImageFormView = new BackgroundImageFormView({ el: '.background-field', model: this.model });
            this.backgroundImageFormView = backgroundImageFormView;
            backgroundImageFormView.render();

            this.categoriesListSelectView = this.$el.find('#Categories').multiSelect({
                selectAll: false,
                listHeight: 260,
                messageText: 'Select more than one category if required',
                noOptionsText: 'No Categories',
                noneSelected: '<span class="default-option">Select Categories</span>',
                oneOrMoreSelected: function (selectedOptions) {
                    var $selectedHtml = $('<span />');
                    var cats = [];
                    _.each(selectedOptions, function (option) {
                        cats.push(option.text);
                    });
                    $selectedHtml.append(cats.join(', '));
                    return $selectedHtml;
                }
            });
        },

        _contentChanged: function (e) {
            var target = $(e.currentTarget);
            var data = {};
            data[target.attr('id')] = target.attr('value');
            this.model.set(data);
        },

        _categoriesChanged: function (e) {
            var $checkbox = $(e.currentTarget);
            if ($checkbox.attr('checked') === 'checked') {
                var category = $checkbox.attr('value');
                this.model.addCategory(category);
            } else {
                this.model.removeCategory($checkbox.attr('value'));
            }
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

            this.$el.find('#Name, #Categories, #Description').removeClass('input-validation-error');

            if (_.any(errors, function (item) { return item.Field === 'Name'; })) {
                this.$el.find('#Name').addClass('input-validation-error');
            }
            if (_.any(errors, function (item) { return item.Field === 'Categories'; })) {
                this.$el.find('#Categories').addClass('input-validation-error');
            }
            if (_.any(errors, function (item) { return item.Field === 'Description'; })) {
                this.$el.find('#Description').addClass('input-validation-error');
            }
        },

        _cancel: function () {
            app.showPreviousContentView();
        },

        _save: function () {
            if (this.avatarImageFormView.isUploading || this.backgroundImageFormView.isUploading) {
                return;
            }

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

    return ProjectFormView;
});