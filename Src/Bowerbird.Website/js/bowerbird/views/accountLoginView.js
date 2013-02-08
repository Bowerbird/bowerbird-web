/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// AccountLoginView
// ----------------

define(['jquery', 'underscore', 'backbone', 'ich', 'app'], function ($, _, Backbone, ich, app) {

    var AccountLoginView = Backbone.Marionette.Layout.extend({
        className: 'login single',

        template: 'AccountLogin',

        events: {
            'click #login': '_login',
            'change #Email': '_contentChanged',
            'change #Password': '_contentChanged',
            'change #RememberMe': '_rememberMeChanged'
        },

        initialize: function (options) {
            this.model.on('validated', this.onValidation, this);
        },

        serializeData: function () {
            return {
                Model: {
                    AccountLogin: this.model.toJSON()
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
        },

        onValidation: function (obs, errors) {
            if (errors.length == 0) {
                this.$el.find('.validation-summary').slideUp(function () { $(this).remove(); });
            }

            if (errors.length > 0) {
                if (this.$el.find('.validation-summary').length == 0) {
                    this.$el.find('.form').prepend(ich.ValidationSummary({
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

            this.$el.find('#Email, #Password').removeClass('input-validation-error');

            if (_.any(errors, function (item) { return item.Field === 'Email'; })) {
                this.$el.find('#Email').addClass('input-validation-error');
            }
            if (_.any(errors, function (item) { return item.Field === 'Password'; })) {
                this.$el.find('#Password').addClass('input-validation-error');
            }
        },

        _contentChanged: function (e) {
            var target = $(e.currentTarget);
            var data = {};
            data[target.attr('id')] = target.attr('value');
            this.model.set(data);
        },

        _rememberMeChanged: function (e) {
            this.model.set('RememberMe', e.currentTarget.checked);
        },

        _login: function (e) {
            e.preventDefault();

            this.$el.find('#login').attr('disabled', 'disabled').val('Authenticating...');

            var that = this;

            this.model.save(null, {
                success: function (model, response, options) {
                    that.$el.find('#login').attr('disabled', 'disabled').val('Loading...');
                    that.onValidation(that.model, []);
                    window.location.replace(that.model.get('ReturnUrl'));
                },
                error: function (model, xhr, options) {
                    that.$el.find('#login').removeAttr('disabled').val('Login');

                    var data = JSON.parse(xhr.responseText);
                    that.onValidation(that.model, data.Model.Errors);
                    $('html, body').animate({ scrollTop: 0 }, 'slow');
                }
            });

            return false;
        }
    });

    return AccountLoginView;

}); 