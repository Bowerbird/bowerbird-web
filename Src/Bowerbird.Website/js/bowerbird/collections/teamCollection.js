/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// TeamCollection
// -----------------

define(['jquery', 'underscore', 'backbone', 'app', 'models/team'], function ($, _, Backbone, app, Team) {

    var TeamCollection = Backbone.Collection.extend({
        model: Team,

        url: '/teams',

        initialize: function () {
            _.extend(this, Backbone.Events);
        },

        toJSONViewModel: function () {
            var viewModels = [];
            _.each(this.models, function (team) {
                viewModels.push(team.toJSONViewModel());
            });
            return viewModels;
        }
    });

    return TeamCollection;

});