/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// OrganisationCollection
// -----------------

define(['jquery', 'underscore', 'backbone', 'models/organisation'], function ($, _, Backbone, Organisation) {

    var OrganisationCollection = Backbone.Collection.extend({
        model: Organisation,

        url: '/organisations',

        initialize: function () {
            _.extend(this, Backbone.Events);
        },

        toJSONViewModel: function () {
            var viewModels = [];
            _.each(this.models, function (organisation) {
                viewModels.push(organisation.toJSONViewModel());
            });
            return viewModels;
        }
    });

    return OrganisationCollection;

});