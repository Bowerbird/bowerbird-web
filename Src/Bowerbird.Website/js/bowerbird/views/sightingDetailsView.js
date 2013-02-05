/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// SightingDetailsView
// -------------------

define(['jquery', 'underscore', 'backbone', 'ich', 'app', 'moment', 'voter', 'timeago', 'tipsy'],
function ($, _, Backbone, ich, app, moment, Voter) {

    var SightingDetailsView = Backbone.Marionette.ItemView.extend({

        events: {
            'click .thumbnails > div': 'showMedia',
            'click .add-identification-button': 'showIdentificationForm',
            'click .add-note-button': 'showNoteForm',
            'click .view-button': 'showItem',
            'click .edit-button': 'showItem',
            'click h3 a': 'showItem',
            'click .sighting-actions .vote-up': 'voteUp',
            'click .sighting-actions .vote-down': 'voteDown',
            'click .sighting-actions .favourites-button': 'addToFavourites'
        },

        initialize: function (options) {
            _.bindAll(this, 'refresh', 'showMedia', 'showNoteForm', 'showIdentificationForm', 'voteUp', 'voteDown');

            if (!options.template) {
                this.template = 'SightingTileDetails';
            } else {
                this.template = options.template;
            }

            if (options.viewType) {
                this.viewType = options.viewType;
            }

            this.isObservationActviityItem = options.isObservationActviityItem;

            this.showLocation = false;
            this.showThumbnails = false;

            if (this.template !== 'SightingTileDetails') {
                this.showLocation = true;
                this.showThumbnails = true;
            }
        },

        serializeData: function () {
            var viewModel = this.model.toJSON();
            viewModel.ObservedOnDescription = moment(this.model.get('ObservedOn')).format('D MMM YYYY');
            viewModel.CreatedOnDescription = moment(this.model.get('CreatedOn')).format('D MMM YYYY');
            viewModel.ObservationAdded = this.isObservationActviityItem; // For category icon selection in template
            viewModel.Model = { AuthenticatedUser: app.authenticatedUser != undefined }; 
            return viewModel;
        },

        currentObservationMedia: null,

        onRender: function () {
            this._showDetails();
        },

        showBootstrappedDetails: function () {
            this._showDetails();
        },

        _showDetails: function () {
            this.currentObservationMedia = _.find(this.model.get('Media'), function (item) {
                return item.IsPrimaryMedia;
            });

            var resizeTimer;
            var refresh = this.refresh;
            $(window).resize(function () {
                clearTimeout(resizeTimer);
                resizeTimer = setTimeout(function () {
                    refresh();
                }, 100);
            });

            if (this.showLocation) {
                var mapOptions = {
                    zoom: 9,
                    center: new google.maps.LatLng(this.model.get('Latitude'), this.model.get('Longitude')),
                    disableDefaultUI: true,
                    scrollwheel: false,
                    disableDoubleClickZoom: false,
                    draggable: false,
                    keyboardShortcuts: false,
                    mapTypeId: google.maps.MapTypeId.TERRAIN
                };

                var map = new google.maps.Map(this.$el.find('.map').get(0), mapOptions);
                this.map = map;

                var point = new google.maps.LatLng(Number(this.model.get('Latitude')), Number(this.model.get('Longitude')));
                this.point = point;

                var image = new google.maps.MarkerImage('/img/map-pin.png',
                    new google.maps.Size(43, 38),
                    new google.maps.Point(0, 0)
                );

                var shadow = new google.maps.MarkerImage('/img/map-pin-shadow.png',
                    new google.maps.Size(59, 32),
                    new google.maps.Point(0, 0),
                    new google.maps.Point(17, 32)
                );

                this.mapMarker = new google.maps.Marker({
                    position: point,
                    map: map,
                    clickable: false,
                    draggable: false,
                    icon: image,
                    shadow: shadow
                });
            }

            if (app.authenticatedUser && this.template !== 'SightingTileDetails') {
                this.$el.find('.vote-up, .vote-down, .add-identification-button, .add-note-button, .add-comment-button').tipsy({ gravity: 'n', html: true });
                this.$el.find('.favourites-button, .edit-button').tipsy({ gravity: 's', html: true });
            }
        },

        refresh: function () {
            // Resize video and audio in observations
            if (this.currentObservationMedia) {
                var newWidth = (600 / 800) * this.$el.find('.preview').width();
                this.$el.find('.preview .video-media, .preview .video-media > iframe, .preview .audio-media.media-constrained-600, .preview .image-media').height(newWidth + 'px');
            }

            if (this.showLocation) {
                // Resize maps in sightings
                this.map.panTo(this.point);
                this.$el.find('.map').width(this.$el.find('.location').width() + 'px');
                google.maps.event.trigger(this.map, 'resize');
                this.map.panTo(this.point);
            }
        },

        showMedia: function (e) {
            var index = this.$el.find('.thumbnails > div').index(e.currentTarget);
            this.currentObservationMedia = this.model.get('Media')[index];
            var descriptionHtml = '';
            this.$el.find('.preview').empty().append(ich.MediaConstrained600(this.currentObservationMedia.MediaResource));
            if (this.currentObservationMedia.Description && this.currentObservationMedia.Description !== '') {
                descriptionHtml = '<div class="media-details"><div class="overlay"></div><p class="description">' + this.currentObservationMedia.Description + '</p></div>';
                this.$el.find('.preview').append(descriptionHtml);
            }
            this.refresh();
        },

        showItem: function (e) {
            e.preventDefault();
            this.$el.find('.actions a').tipsy.revalidate();
            Backbone.history.navigate($(e.currentTarget).attr('href'), { trigger: true });
        },

        showIdentificationForm: function (e) {
            e.preventDefault();
            this.$el.find('.actions a').tipsy.revalidate();
            Backbone.history.navigate($(e.currentTarget).attr('href'), { trigger: true });
        },

        showNoteForm: function (e) {
            e.preventDefault();
            this.$el.find('.actions a').tipsy.revalidate();
            Backbone.history.navigate($(e.currentTarget).attr('href'), { trigger: true });
        },

        voteUp: function (e) {
            Voter.voteUp(this.model);
            this.updateVotePanel('up');
        },

        voteDown: function (e) {
            Voter.voteDown(this.model);
            this.updateVotePanel('down');
        },

        addToFavourites: function (e) {
            Voter.addToFavourites(this.model);
            this.updateFavouritesPanel();
        },

        updateFavouritesPanel: function () {
            var $favouritesPanel = this.$el.find('.favourites-panel');
            $favouritesPanel.find('.favourites-count').text(this.model.get('FavouritesCount'));
            $favouritesPanel.find('.favourites-button').toggleClass('selected');
        },

        updateVotePanel: function (direction) {
            var $votePanel = this.$el.find('.vote-panel');
            if (direction === 'up') {
                $votePanel.find('.vote-down').removeClass().addClass('vote-down button');
                $votePanel.find('.vote-up').removeClass().addClass('vote-up button user-vote-score' + this.model.get('UserVoteScore'));
                $votePanel.find('.vote-score').removeClass().addClass('vote-score user-vote-score' + this.model.get('UserVoteScore')).text(this.model.get('TotalVoteScore'));
            } else {
                $votePanel.find('.vote-up').removeClass().addClass('vote-up button');
                $votePanel.find('.vote-down').removeClass().addClass('vote-down button user-vote-score' + this.model.get('UserVoteScore'));
                $votePanel.find('.vote-score').removeClass().addClass('vote-score user-vote-score' + this.model.get('UserVoteScore')).text(this.model.get('TotalVoteScore'));
            }
        }

    });

    return SightingDetailsView;

});