/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// OrganisationController
// ----------------------

// This is the controller contributions (observations & posts). It contains all of the 
// high level knowledge of how to run the app when it's in contribution mode.
define(['jquery', 'underscore', 'backbone', 'app', 'models/team', 'views/organisationformlayoutview'], function ($, _, Backbone, app, Organisation, OrganisationFormLayoutView) {

    var OrganisationController = {};

    // OrganisationController Public API
    // ---------------------------------

    // Show an organisation
    OrganisationController.showOrganisationForm = function () {

        var organisationFormLayoutView = new OrganisationFormLayoutView({ el: $('.organisation-create-form'), model: new Organisation(app.prerenderedView.Organisation) });

        organisationFormLayoutView.render();

        app.prerenderedView.isBound = true;
    };

    // OrganisationController Event Handlers
    // -------------------------------------

    app.vent.on('organisation:show', function () {
        OrganisationController.showOrganisationForm();
    });

    return OrganisationController;

});