/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ObservationMediaItemView
// ------------------------

define(['jquery', 'underscore', 'backbone', 'app', 'ich', 'views/editobservationmediaformview'],
function ($, _, Backbone, app, ich, EditObservationMediaFormView) {
    var ImageProvider = function (options) {

    };

    var VideoProvider = function (options) {

    };

    var ObservationMediaItemView = Backbone.Marionette.ItemView.extend({
        className: 'observation-media-item',

        events: {
            'click .sub-menu-button': '_showMenu',
            'click .sub-menu-button li': '_selectMenuItem',
            'click .view-menu-item': '_viewMedia',
            'click .edit-menu-item': '_editMediaDetails',
            'click .remove-menu-item': '_removeMedia'
        },

        template: 'ObservationMediaItem',

        provider: null,

        initialize: function (options) {
            var mediaType = this.model.get('MediaType');
            if (mediaType === 'image') {
                this.provider = new ImageProvider();
            } else if (mediaType === 'video') {
                this.provider = new VideoProvider();
            }
        },

        onRender: function () {
            this.$el.css({ position: 'absolute', top: '-250px', width: 280 + 'px' });
            return this;
        },

        _showMenu: function (e) {
            $('.sub-menu-button').removeClass('active');
            $(e.currentTarget).addClass('active');
            e.stopPropagation();
        },

        _selectMenuItem: function (e) {
            $('.sub-menu-button').removeClass('active');
            e.stopPropagation();
        },

        _viewMedia: function (e) {
            e.preventDefault();
        },

        _editMediaDetails: function (e) {
            e.preventDefault();
            $('body').append('<div id="modal-dialog"></div>');
            log(this.model);
            var editObservationMediaFormView = new EditObservationMediaFormView({ el: $('#modal-dialog'), model: this.model });
            editObservationMediaFormView.on('editmediadone', this._onEditMedia, this);
            editObservationMediaFormView.render();
        },

        _onEditMedia: function (description, licence) {
            log('editmediadone', description, licence);
            this.trigger('detailsedited', { mediaResource: this.model, description: description, licence: licence });
        },

        _removeMedia: function (e) {
            e.preventDefault();
            this.trigger('removemedia', this);
        }
    });

    return ObservationMediaItemView;

});