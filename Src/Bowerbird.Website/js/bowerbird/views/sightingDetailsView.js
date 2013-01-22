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
            'click .edit-observation-button': 'showItem',
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

            this.showLocation = false;
            this.showThumbnails = false;

            if (this.template == 'SightingFullFullDetails' || this.template == 'SightingListItem' || this.template == 'SightingSummaryDetails') {
                this.showLocation = true;
                this.showThumbnails = true;
            }
        },

        serializeData: function () {
            var viewModel = this.model.toJSON();
            viewModel.ObservedOnDescription = moment(this.model.get('ObservedOn')).format('D MMM YYYY');
            viewModel.CreatedOnDescription = moment(this.model.get('CreatedOn')).format('D MMM YYYY');
            return viewModel;
        },

        currentObservationMedia: null,

        onShow: function () {
            this._showDetails();
            //this.refresh();
        },

        onRender: function () {
            this._showDetails();
            //this.refresh();
        },

        showBootstrappedDetails: function () {
            this._showDetails();
        },

        _showDetails: function () {
            this.currentObservationMedia = _.find(this.model.get('Media'), function (item) {
                return item.IsPrimaryMedia;
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

            var resizeTimer;
            var refresh = this.refresh;
            $(window).resize(function () {
                clearTimeout(resizeTimer);
                resizeTimer = setTimeout(function () {
                    refresh();
                }, 100);
            });

            this.$el.find('.actions .button').tipsy({ gravity: 's', html: true });

            if (this.model.get('User').Id !== app.authenticatedUser.user.id) {
                this.$el.find('.edit-observation-button').hide();
            }

            this.$el.find('.vote-up, .vote-down, .add-identification-button, .add-note-button, .add-comment-button').tipsy({ gravity: 'n', html: true });
            this.$el.find('.favourites-button').tipsy({ gravity: 's', html: true });
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

            this.$el.find('.vote-panel .vote-down').removeClass().addClass('vote-down button');
            this.$el.find('.vote-panel .vote-up').removeClass().addClass('vote-up button user-vote-score' + this.model.get('UserVoteScore'));
            this.$el.find('.vote-panel .vote-score').removeClass().addClass('vote-score user-vote-score' + this.model.get('UserVoteScore')).text(this.model.get('TotalVoteScore'));
        },

        voteDown: function (e) {
            Voter.voteDown(this.model);

            this.$el.find('.vote-panel .vote-up').removeClass().addClass('vote-up button');
            this.$el.find('.vote-panel .vote-down').removeClass().addClass('vote-down button user-vote-score' + this.model.get('UserVoteScore'));
            this.$el.find('.vote-panel .vote-score').removeClass().addClass('vote-score user-vote-score' + this.model.get('UserVoteScore')).text(this.model.get('TotalVoteScore'));
        },

        addToFavourites: function (e) {
            Voter.addToFavourites(this.model);

            this.$el.find('.favourites-panel .favourites-count').text(this.model.get('FavouritesCount'));
            this.$el.find('.favourites-panel .favourites-button').toggleClass('selected');
        }

    });

    return SightingDetailsView;

});