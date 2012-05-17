/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ObservationLayoutView
// ---------------------

// Layout of an observation in both edit and view mode
define(['jquery', 'underscore', 'backbone', 'app'], function ($, _, Backbone, app) {

    //    var DetailsRegionManager = Backbone.Marionette.Region.extend({
    //        el: '.details'
    //    });

    var ObservationLayoutView = Backbone.Marionette.Layout.extend({
//        tagName: 'section',

//        id: 'content',

        //        className: 'observation',

        className: 'form single-medium observation',

        template: 'Observation',

        regions: {
            main: '.main',
            notes: '.notes',
            comments: '.comments'
        },

        showBootstrappedDetails: function () {
            this.initializeRegions();
        }
    });

    return ObservationLayoutView;

});