/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// OrganisationController
// ----------------------
define(['jquery','underscore','backbone','app','models/team','views/organisationformlayoutview'],
function ($,_,Backbone,app,Organisation,OrganisationFormLayoutView) 
{
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

// OrganisationRouter
// ------------------
define(['jquery', 'underscore', 'backbone', 'app', 'controllers/organisationcontroller'],
function ($, _, Backbone, app, OrganisationController) 
{
    var OrganisationRouter = Backbone.Marionette.AppRouter.extend({
        appRoutes: {
            'organisations/create': 'showOrganisationForm'
        }
    });

    app.addInitializer(function () {
        this.organisationRouter = new OrganisationRouter({
            controller: OrganisationController
        });
    });
});