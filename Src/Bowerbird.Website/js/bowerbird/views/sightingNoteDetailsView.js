/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// SightingNoteDetailsView
// -----------------------

define(['jquery', 'underscore', 'backbone', 'ich', 'app', 'moment', 'voter', 'timeago', 'tipsy'],
function ($, _, Backbone, ich, app, moment, Voter) {

    var SightingNoteDetailsView = Backbone.Marionette.ItemView.extend({
        template: 'ActivityItemSightingNoteAdded',

        events: {
            'click h3 a': 'showSighting',
            'click .vote-panel .vote-up': 'voteUp',
            'click .vote-panel .vote-down': 'voteDown'
        },

        initialize: function (options) {
            this.sighting = options.sighting;
        },

        serializeData: function () {
            var viewModel = {
                SightingNote: this.model.toJSON(),
                Sighting: this.sighting.toJSON()
            };
            return viewModel;
        },

        currentObservationMedia: null,

        onShow: function () {
            this._showDetails();
        },

        onRender: function () {
            this._showDetails();
        },

        showBootstrappedDetails: function () {
            this._showDetails();
        },

        _showDetails: function () {
            this.$el.find('.vote-up, .vote-down, .add-comment-button').tipsy({ gravity: 'n', html: true });
        },

//        showNoteForm: function (e) {
//            e.preventDefault();
//            this.$el.find('.observation-action-menu a').tipsy.revalidate();
//            Backbone.history.navigate($(e.currentTarget).attr('href'), { trigger: true });
//        },

        showSighting: function (e) {
            e.preventDefault();
            Backbone.history.navigate($(e.currentTarget).attr('href'), { trigger: true });
        },

        voteUp: function (e) {
            log('voting!');
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

    return SightingNoteDetailsView;

});