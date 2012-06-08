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

    var getModel = function (id) {
        var deferred = new $.Deferred();

        if (app.isPrerendering('organisations')) {
            deferred.resolve(app.prerenderedView.data);
        } else {
            var params = {};
            if (id) {
                params['id'] = id;
            }
            $.ajax({
                url: '/organisations/create',
                data: params
            }).done(function (data) {
                deferred.resolve(data.Model);
            });
        }
        return deferred.promise();
    };

    // OrganisationController Public API
    // ---------------------------------

    // Show an organisation
    OrganisationController.showOrganisationForm = function (id) {
        log('organisationController:showOrganisationForm');
        $.when(getModel(id))
            .done(function (model) {
                var organisation = new Organisation(model.Organisation);
                var organisationFormLayoutView = new OrganisationFormLayoutView({ model: organisation });

                app.content[app.getShowViewMethodName('organisations')](organisationFormLayoutView);

                if (app.isPrerendering('organisations')) {
                    organisationFormLayoutView.showBootstrappedDetails();
                }

                app.setPrerenderComplete();
            });
    };

    return OrganisationController;

});