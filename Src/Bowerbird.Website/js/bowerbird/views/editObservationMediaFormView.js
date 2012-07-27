/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// EditObservationMediaFormView
// ----------------------------

define(['jquery', 'underscore', 'backbone', 'app', 'ich', 'multiselect'],
function ($, _, Backbone, app, ich) {

    var EditObservationMediaFormView = Backbone.Marionette.ItemView.extend({
        id: 'edit-observation-media-form',

        template: 'EditObservationMediaForm',

        events: {
            'click .cancel-button': '_cancel',
            'click .close': '_cancel',
            'click .done-button': '_done'
        },

        licenceDetails: [
              {
                  Name: 'None (All Rights Reserved)',
                  Id: ' ',
                  Description: 'The original creator/s of the work are recognized as the legal owner/s when it comes to fair use of the work by others. The creator holds all of the legal rights to the work unless or until they decide to grant some of those rights to others.',
                  Icons: ['/img/copyright.svg']
              },
              {
                  Name: 'Attribution',
                  Id: 'BY',
                  Description: 'This licence lets others distribute, remix and build upon a work, even commercially, as long as they credit the original creator/s (and any other nominated parties). This is the most accommodating of the licences in terms of what others can do with the work.',
                  DeedUri: 'http://creativecommons.org/licenses/by/3.0/au',
                  LegalCodeUri: 'http://creativecommons.org/licenses/by/3.0/au/legalcode',
                  Icons: ['/img/by.svg']
              },
              {
                  Name: 'Attribution-Share Alike',
                  Id: 'BY-SA',
                  Description: 'This licence lets others distribute, remix and build upon the work, even for commercial purposes, as long as they credit the original creator/s (and any other nominated parties) and license any new creations based on the work under the same terms. All new derivative works will carry the same licence, so will also allow commercial use.<br />' +
                        'In other words, you agree to share your materials with others, if they will share their new works in return. This licence is often compared to the free software licences, known as ‘copyleft.’',
                  DeedUri: 'http://creativecommons.org/licenses/by-sa/3.0/au',
                  LegalCodeUri: 'http://creativecommons.org/licenses/by-sa/3.0/au/legalcode',
                  Icons: ['/img/by.svg', '/img/sa.svg']
              },
              {
                  Name: 'Attribution-No Derivative Works',
                  Id: 'BY-ND',
                  Description: 'This licence allows others to distribute the work, even for commercial purposes, as long as the work is unchanged, and the original creator/s (and any other nominated parties) are credited.',
                  DeedUri: 'http://creativecommons.org/licenses/by-nd/3.0/au',
                  LegalCodeUri: 'http://creativecommons.org/licenses/by-nd/3.0/au/legalcode',
                  Icons: ['/img/by.svg', '/img/nd.svg']
              },
              {
                  Name: 'Attribution-Noncommercial',
                  Id: 'BY-NC',
                  Description: 'This licence lets others distribute, remix and build upon the work, but only if it is for non-commercial purposes and they credit the original creator/s (and any other nominated parties). They don’t have to license their derivative works on the same terms.',
                  DeedUri: 'http://creativecommons.org/licenses/by-nc/3.0/au',
                  LegalCodeUri: 'http://creativecommons.org/licenses/by-nc/3.0/au/legalcode',
                  Icons: ['/img/by.svg', '/img/nc.svg']
              },
              {
                  Name: 'Attribution-Noncommercial-Share Alike',
                  Id: 'BY-NC-SA',
                  Description: 'This licence lets others distribute, remix and build upon the work, but only if it is for non-commercial purposes, they credit the original creator/s (and any other nominated parties) and they license their derivative works under the same terms.',
                  DeedUri: 'http://creativecommons.org/licenses/by-nc-sa/3.0/au',
                  LegalCodeUri: 'http://creativecommons.org/licenses/by-nc-sa/3.0/au/legalcode',
                  Icons: ['/img/by.svg', '/img/nc.svg', '/img/sa.svg']
              },
              {
                  Name: 'Attribution-Noncommercial-No Derivatives',
                  Id: 'BY-NC-ND',
                  Description: 'This licence is the most restrictive of the six main licences, allowing redistribution of the work in its current form only. This licence is often called the ‘free advertising’ licence because it allows others to download and share the work as long as they credit the original creator/s (and any other nominated parties), they don’t change the material in any way and they don’t use it commercially.',
                  DeedUri: 'http://creativecommons.org/licenses/by-nc-nd/3.0/au',
                  LegalCodeUri: 'http://creativecommons.org/licenses/by-nc-nd/3.0/au/legalcode',
                  Icons: ['/img/by.svg', '/img/nc.svg', '/img/nd.svg']
              }
        ],

        serializeData: function () {
            return {
                Model: {
                    Media: this.model.toJSON(),
                    Licences: _.map(this.licenceDetails, function (licence) {
                        return {
                            Text: licence.Name,
                            Value: licence.Id,
                            Selected: licence.Id === 'BY' ? true : false
                        };
                    })
                }
            };
        },

        initialize: function (options) {
            //            _.bindAll(this, '_loadVideo', '_onGetYouTubeVideo', '_onGetVimeoVideo', '_onGetVideoError', '_updateVideoStatus');

            //            if (options.videoProviderName === 'youtube') {
            //                this.provider = new YouTubeVideoProvider({ onGetVideoSuccess: this._onGetYouTubeVideo, onGetVideoError: this._onGetVideoError });
            //            } else if (options.videoProviderName === 'vimeo') {
            //                this.provider = new VimeoVideoProvider({ onGetVideoSuccess: this._onGetVimeoVideo, onGetVideoError: this._onGetVideoError });
            //            }
        },

        onRender: function () {
            var that = this;
            var defaultLicence = _.find(that.licenceDetails, function(item) { return item.Id === ' '; });
            this.licenceListSelectView = this.$el.find("#Licence").multiSelect({
                selectAll: false,
                listHeight: 263,
                singleSelect: true,
                noOptionsText: 'No Licences',
                noneSelected: '<span>' + defaultLicence.Name + '</span><img src="' + defaultLicence.Icons[0] + '" alt="" />',
                renderOption: function (id, option) {
                    var licence = _.find(that.licenceDetails, function (item) {
                        return item.Id === option.value;
                    });
                    var model = {
                        Model: {
                            Licence: licence,
                            Selected: option.selected
                        }
                    };
                    return ich.LicenceItem(model);
                },
                oneOrMoreSelected: function (selectedOptions) {
                    var $selectedHtml = $('<span />');
                    _.each(selectedOptions, function (option) {
                        var licence = _.find(that.licenceDetails, function (item) {
                            return item.Id === option.value;
                        });
                        $selectedHtml.append('<span>' + licence.Name + '</span> ');
                        for(var x = 0; x < licence.Icons.length; x++) {
                            $selectedHtml.append('<img src="' + licence.Icons[x] + '" alt="" />');
                        }
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
            this.trigger('editmediadone', this.$el.find('#Description').val(), this.$el.find('#Licence').val());
            this.remove();
            //            if (this.videoId !== '') {
            //                this.trigger('videouploaded', this.videoId, this.provider.getJSON().name);
            //                this.remove();
            //            }
        }
    });

    return EditObservationMediaFormView;
});