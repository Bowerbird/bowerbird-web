/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// MediaFormView
// -------------

define(['jquery', 'underscore', 'backbone', 'app', 'ich', 'licences', 'multiselect'],
function ($, _, Backbone, app, ich, licences) {

    var MediaFormView = Backbone.Marionette.ItemView.extend({
        id: 'edit-observation-media-form',

        template: 'MediaForm',

        events: {
            'click .cancel-button': '_cancel',
            'click .close': '_cancel',
            'click .done-button': '_done'
        },

        serializeData: function () {
            return {
                Model: {
                    Media: this.model.toJSON(),
                    Licences: _.map(licences.getAll(), function (licence) {
                        return {
                            Text: licence.Name,
                            Value: licence.Id,
                            Selected: licence.Id === this.selectedLicence
                        };
                    },
                    this)
                }
            };
        },

        initialize: function (options) {
            _.bindAll(this, 'onRender');
            this.selectedLicence = this.model.get('Licence');
        },

        onRender: function () {
            var that = this;
            var defaultLicence = licences.get(' ');
            this.licenceListSelectView = this.$el.find("#Licence").multiSelect({
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

            return this;
        },

        _cancel: function () {
            this.remove();
        },

        _done: function () {
            this.model.set({ Description: this.$el.find('#Description').val(), Licence: this.selectedLicence });
            this.trigger('editmediadone', this.model);
            this.remove();
        }
    });

    return MediaFormView;
});