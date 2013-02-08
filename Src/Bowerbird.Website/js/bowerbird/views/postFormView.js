/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// PostFormView
// ------------

define(['jquery', 'underscore', 'backbone', 'app', 'ich', 'moment', 'datepicker', 'multiselect', 'jqueryui/dialog'],
function ($, _, Backbone, app, ich, moment) {

    var postTypes = [
        {
            Text: 'General news',
            Value: 'news'
        },
        {
            Text: 'Meeting',
            Value: 'meeting'
        },
        {
            Text: 'Newsletter',
            Value: 'newsletter'
        }];
    
    var PostFormView = Backbone.Marionette.Layout.extend({

        viewType: 'form',

        className: 'form form-medium single post-form',

        template: 'PostForm',

        regions: {
            media: '#media-resources-fieldset'
        },

        events: {
            'click #cancel': '_cancel',
            'click #save': '_save',
            'change input#Subject': '_contentChanged',
            'change textarea#Message': '_contentChanged',
            'change #posttype-field input:checkbox': '_postTypeChanged'
        },

        initialize: function (options) {
            if (this.model.id) {
                this.viewEditMode = 'update';
            } else {
                this.viewEditMode = 'create';
            }
        },

        serializeData: function () {
            return {
                Model: {
                    Post: this.model.toJSON(),
                    PostTypeSelectList: _.map(postTypes, function(item) {
                        return {
                            Value: item.Value,
                            Text: item.Text,
                            Selected: item.Value === this.model.get('PostType')
                        };
                    }, this),
                    Create: this.viewEditMode === 'create',
                    Update: this.viewEditMode === 'update'
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
//            var postMediaFormView = new PostMediaFormView({ el: '#media-details', model: this.model, collection: this.model.media });
//            this.media.attachView(postMediaFormView);
//            postMediaFormView.render();

            this.postTypeListSelectView = this.$el.find('#PostType').multiSelect({
                selectAll: false,
                listHeight: 260,
                singleSelect: true,
                noOptionsText: 'No Types',
                noneSelected: '<span class="default-option">Select Type</span>',
                oneOrMoreSelected: function (selectedOptions) {
                    var $selectedHtml = $('<span />');
                    _.each(selectedOptions, function (option) {
                        $selectedHtml.append('<span>' + option.text + '</span> ');
                    });
                    return $selectedHtml.children();
                }
            });

            this.$el.find('#Subject, #PostType, #Message').tipsy({ trigger: 'focus', gravity: 'w', fade: true, live: true });
        },

        _contentChanged: function (e) {
            var target = $(e.currentTarget);
            var data = {};
            data[target.attr('id')] = target.attr('value');
            this.model.set(data);
        },

        _postTypeChanged: function (e) {
            var postType = '';
            var $checkbox = $(e.currentTarget);
            if ($checkbox.attr('checked') === 'checked') {
                postType = $checkbox.attr('value');
            }
            this.model.set('PostType', postType);
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

            if (_.any(errors, function (item) { return item.Field === 'Subject'; })) {
                this.$el.find('#Subject').addClass('input-validation-error');
            }
            if (_.any(errors, function (item) { return item.Field === 'PostType'; })) {
                this.$el.find('#PostType').addClass('input-validation-error');
            }
            if (_.any(errors, function (item) { return item.Field === 'Message'; })) {
                this.$el.find('#Message').addClass('input-validation-error');
            }
        },

        _cancel: function () {
            app.showPreviousContentView();
        },

        _save: function () {
//            if (this.media.currentView.progressCount > 0) {
//                return;
//            }

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

    return PostFormView;

});