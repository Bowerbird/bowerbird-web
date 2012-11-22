/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// UserFormView
// ------------

define(['jquery', 'underscore', 'backbone', 'app', 'ich', 'loadimage', 'views/editavatarview', 'licences', 'fileupload', 'multiselect'],
function ($, _, Backbone, app, ich, loadImage, EditAvatarView, licences) {
    var UserFormView = Backbone.Marionette.Layout.extend({

        viewType: 'form',

        className: 'form form-medium single user-form',

        template: 'UserForm',

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

        //        serializeData: function () {
        ////            var licence = licences.get(this.model.get('Licence'));
        ////            return {
        ////                Description: this.model.get('Description'),
        ////                LicenceName: licence.Name,
        ////                LicenceIcons: licence.Icons,
        ////                IsPrimaryMedia: this.model.get('IsPrimaryMedia'),
        ////                MediaResource: this.model.mediaResource.toJSON()
        ////            };            

        //            return {
        //                Model: {
        //                    User: this.model.toJSON(),
        //                    TimezoneSelectList: this.timezoneSelectList,
        //                    LicenceSelectList: this.licenceSelectList,
        //                    Licences: _.map(licences.getAll(), function (licence) {
        //                        return {
        //                            Text: licence.Name,
        //                            Value: licence.Id,
        //                            Selected: licence.Id === this.model.get('DefaultLicence')
        //                        };
        //                    },
        //                    this)
        //                }
        //            };
        //        },

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
                noneSelected: '<span>' + defaultLicence.Name + '</span><img src="' + defaultLicence.Icons[0] + '" alt="" />',
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
            log('userFormLayoutView:_contentChanged');
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

        _cancel: function () {
            app.showPreviousContentView();
        },

        _save: function () {
            this.model.set('Id', this.model.id.replace('users/', ''));
            this.model.save({}, {
                success: function (model, response, options) {
                    window.location.replace('/');
                }
            });
        }
    });

    return UserFormView;
});