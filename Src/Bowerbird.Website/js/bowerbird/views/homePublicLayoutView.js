/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// HomePublicLayoutView
// --------------------

// The home page view when not logged in
define(['jquery', 'underscore', 'backbone', 'app'], function ($, _, Backbone, app) {

    var HomePublicLayoutView = Backbone.Marionette.Layout.extend({
        className: 'home',

        template: 'Home',

//        regions: {
//            summary: '.summary',
//            details: '.details'
//        },

        showBootstrappedDetails: function () {
            this.initializeRegions();
            this.$el = $('#content .home');
        }

//        showStream: function () {
//            var streamItemCollection = new StreamItemCollection();
//            var options = {
//                model: app.authenticatedUser.user,
//                collection: streamItemCollection
//            };

//            if (app.isPrerendering('home')) {
//                options['el'] = '.stream';
//            }

//            var streamView = new StreamView(options);

//            if (app.isPrerendering('home')) {
//                this.details.attachView(streamView);
//                streamView.showBootstrappedDetails();
//            } else {
//                this.details.show(streamView);
//            }

//            streamItemCollection.fetchFirstPage();
//        }
    });

    return HomePublicLayoutView;

}); 