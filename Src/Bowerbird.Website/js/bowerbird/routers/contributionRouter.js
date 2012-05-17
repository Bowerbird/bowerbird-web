/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ContributionRouter
// ------------------

define(['jquery', 'underscore', 'backbone', 'app', 'controllers/contributioncontroller'], function ($, _, Backbone, app, ContributionController) {

    var ContributionRouter = Backbone.Marionette.AppRouter.extend({
        appRoutes: {
            'observations/create': 'showObservationForm',
            'observations/:id/update': 'showObservationForm'
        }
    });

    app.addInitializer(function () {
        this.contributionRouter = new ContributionRouter({
            controller: ContributionController
        });
    });

    return ContributionRouter;

});
