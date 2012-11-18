/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ObservationMediaItemFormView
// ----------------------------

define(['jquery', 'underscore', 'backbone', 'app', 'ich', 'views/mediaformview', 'licences', 'circleplayer'],
function ($, _, Backbone, app, ich, MediaFormView, licences, CirclePlayer) {
    var ImageProvider = function (options) {
        this.start = function () {
        };
    };

    var VideoProvider = function (options) {
        this.start = function () {
        };
    };

    var AudioProvider = function (options) {
        this.id = app.generateGuid();

        this.start = function (model) {
            if (model.mediaResource.get('MediaResourceType') === 'audio') {
                this.audioPlayer = new CirclePlayer('#audio-player-' + this.id,
                    {
                        mp3: model.mediaResource.get('Audio').Constrained600.Uri,
                        m4a: model.mediaResource.get('Audio').Constrained600.Uri
                    },
                    {
                        cssSelectorAncestor: '#audio-player-container-' + this.id,
                        swfPath: '/js/libs/jquery.jplayer',
                        supplied: 'mp3,m4a',
                        wmode: 'window',
                        //errorAlerts: true,
                        solution: 'html,flash'
                    });

                //$("#jplayer_inspector").jPlayerInspector({ jPlayer: $('#audio-player') });
            }
        };
    };

    var ObservationMediaItemFormView = Backbone.Marionette.ItemView.extend({
        className: 'observation-media-item',

        events: {
            'click .sub-menu': '_showMenu',
            'click .sub-menu li': '_selectMenuItem',
            'click .view-menu-item': '_viewMedia',
            'click .edit-menu-item': '_editMediaDetails',
            'click .remove-menu-item': '_removeMedia',
            'click .primary-menu-item': '_setPrimaryMedia'
        },

        template: 'ObservationMediaItem',

        provider: null,

        initialize: function () {
            var mediaResourceType = this.model.mediaResource.get('MediaResourceType');
            if (mediaResourceType === 'image') {
                this.provider = new ImageProvider();
            } else if (mediaResourceType === 'video') {
                this.provider = new VideoProvider();
            } else if (mediaResourceType === 'audio') {
                this.provider = new AudioProvider();
            }
            this.model.on('change:IsPrimaryMedia', this._onUpdatePrimaryMedia, this);
        },

        serializeData: function () {
            var licence = licences.get(this.model.get('Licence'));
            return {
                Description: this.model.get('Description'),
                LicenceName: licence.Name,
                LicenceIcons: licence.Icons,
                IsPrimaryMedia: this.model.get('IsPrimaryMedia'),
                MediaResource: this.model.mediaResource.toJSON()
            };
        },

        onRender: function () {
            this.$el.css({ position: 'absolute', top: '-250px', width: 296 + 'px' });
            this.$el.find('.cp-jplayer').attr('id', 'audio-player-' + this.provider.id);
            this.$el.find('.cp-container').attr('id', 'audio-player-container-' + this.provider.id);
            return this;
        },

        start: function () {
            this.provider.start(this.model);
        },

        _showMenu: function (e) {
            $('.sub-menu').removeClass('active');
            $(e.currentTarget).addClass('active');
            e.stopPropagation();
        },

        _selectMenuItem: function (e) {
            $('.sub-menu').removeClass('active');
            e.stopPropagation();
        },

        _viewMedia: function (e) {
            e.preventDefault();
            alert('Coming soon');
        },

        _setPrimaryMedia: function (e) {
            e.preventDefault();
            this.trigger('newprimarymedia', this.model);
        },

        _editMediaDetails: function (e) {
            e.preventDefault();
            $('body').append('<div id="modal-dialog"></div>');

            var mediaFormView = new MediaFormView({ el: $('#modal-dialog'), model: this.model });
            mediaFormView.on('editmediadone', this._onEditMedia, this);
            mediaFormView.render();
        },

        _onEditMedia: function () {
            log('editmediadone');
            var licence = licences.get(this.model.get('Licence'));
            if (this.model.get('Description').trim().length > 0) {
                if (this.$el.find('.description').length === 0) {
                    this.$el.find('.overlay').after('<p class="description" />');
                }
                this.$el.find('.description').text(this.model.get('Description').trim());
            } else {
                this.$el.find('.description').remove();
            }
            this.$el.find('.licence-name').text(licence.Name);
            this.$el.find('.licence-icons img').remove();
            _.forEach(licence.Icons, function (icon) {
                this.$el.find('.licence-icons').append('<img src="' + icon + '" alt="" />');
            }, this);
        },

        _onUpdatePrimaryMedia: function (media) {
            var $elem = this.$el.find('.primary-media');
            if (media.get('IsPrimaryMedia') === true) {
                if ($elem.length === 0) {
                    this.$el.find('.licence-name').after('<p class="primary-media">&bull; Show This Media First</p>');
                }
            } else {
                $elem.remove();
            }
        },

        _removeMedia: function (e) {
            e.preventDefault();
            this.trigger('removemedia', this);
        }
    });

    return ObservationMediaItemFormView;

});