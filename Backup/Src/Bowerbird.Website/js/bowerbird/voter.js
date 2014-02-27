/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />
/// <reference path="../../libs/moment/moment.js" />

// Voter
// -----

define(['jquery', 'underscore', 'backbone', 'app', 'models/sighting', 'models/sightingnote', 'models/identification'],
function ($, _, Backbone, app, Sighting, SightingNote, Identification) {

    var Voter = {};

    var getType = function (model) {
        if (model instanceof Sighting) {
            return 'sighting';
        }
        if (model instanceof SightingNote) {
            return 'sighting-note';
        }
        if (model instanceof Identification) {
            return 'identification';
        }
        return '';
    };

    Voter.voteUp = function (model) {
        var scoreModifier = 0;
        if (model.get('UserVoteScore') === -1) {
            app.vent.trigger('update-' + getType(model) + '-vote', model, 1);
            scoreModifier = 2;
        } else if (model.get('UserVoteScore') === 1) {
            app.vent.trigger('update-' + getType(model) + '-vote', model, 0);
            scoreModifier = -1;
        } else {
            app.vent.trigger('update-' + getType(model) + '-vote', model, 1);
            scoreModifier = 1;
        }
        model.set('UserVoteScore', model.get('UserVoteScore') + scoreModifier);
        model.set('TotalVoteScore', model.get('TotalVoteScore') + scoreModifier);
    };

    Voter.voteDown = function (model) {
        var scoreModifier = 0;
        if (model.get('UserVoteScore') === 1) {
            app.vent.trigger('update-' + getType(model) + '-vote', model, -1);
            scoreModifier = -2;
        } else if (model.get('UserVoteScore') === -1) {
            app.vent.trigger('update-' + getType(model) + '-vote', model, 0);
            scoreModifier = 1;
        } else {
            app.vent.trigger('update-' + getType(model) + '-vote', model, -1);
            scoreModifier = -1;
        }
        model.set('UserVoteScore', model.get('UserVoteScore') + scoreModifier);
        model.set('TotalVoteScore', model.get('TotalVoteScore') + scoreModifier);
    };

    Voter.addToFavourites = function (model) {
        app.vent.trigger('update-favourites', model);

        var favouritesModifier = 0;
        var userFavourite = model.get('UserFavourite');
        if (userFavourite === true) {
            favouritesModifier = -1;
            userFavourite = false;
        } else {
            favouritesModifier = 1;
            userFavourite = true;
        }

        model.set('UserFavourite', userFavourite);
        model.set('FavouritesCount', model.get('FavouritesCount') + favouritesModifier);
    };

    return Voter;
});