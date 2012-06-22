/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// OrganisationController & OrganisationRouter
// -------------------------------------------
define(['jquery', 'underscore', 'backbone', 'app', 'views/organisationlayoutview', 'views/organisationformlayoutview', 'views/organisationcollectionview', 'models/organisation', 'collections/organisationcollection'],
function ($, _, Backbone, app, OrganisationLayoutView, OrganisationFormLayoutView, OrganisationCollectionView, Organisation, OrganisationCollection) {
    var OrganisationRouter = Backbone.Marionette.AppRouter.extend({
        appRoutes: {
            'organisations/explore': 'showOrganisationExplorer',
            'organisations/:id/update': 'showOrganisationForm',
            'organisations/create': 'showOrganisationForm',
            'organisations/:id': 'showOrganisationStream'
        }
    });

    var OrganisationController = {};

    //    app.vent.on('joinOrganisation:', function (organisation) {
    //        OrganisationController.joinOrganisation(organisation);
    //    });

    app.vent.on('organisationAdded:', function (organisation) {
        if (OrganisationController.organisationCollection) {
            OrganisationController.organisationCollection.add(organisation);
        }
    });

    app.vent.on('viewOrganisation:', function (organisation) {
        OrganisationController.showOrganisationStream(organisation.id);
    });

    var getModel = function (id) {
        var deferred = new $.Deferred();
        if (app.isPrerendering('organisations')) {
            deferred.resolve(app.prerenderedView.data);
        } else {
            var params = {};
            if (id) {
                params['id'] = id;
                $.ajax({
                    url: '/organisations/' + id,
                    data: params
                }).done(function (data) {
                    deferred.resolve(data.Model);
                });
            } else {
                $.ajax({
                    url: '/organisations/create'
                }).done(function (data) {
                    deferred.resolve(data.Model);
                });
            }
        }
        return deferred.promise();
    };

    var getExploreList = function (page, pageSize, sortField, sortDirection, searchQuery) {
        var deferred = new $.Deferred();
        if (app.isPrerendering('organisations')) {
            deferred.resolve(app.prerenderedView.data);
        } else {
            var params = {};
            //            if (page) {
            //                params['page'] = page;
            //            }
            //            if (pageSize) {
            //                params['pageSize'] = pageSize;
            //            }
            //            if (sortField) {
            //                params['sortField'] = sortField;
            //            }
            //            if (sortDirection) {
            //                params['sortDirection'] = sortDirection;
            //            }
            //            if (searchQuery) {
            //                params['searchQuery'] = searchQuery;
            //            }
            $.ajax({
                url: '/organisations/explore',
                data: params
            }).done(function (data) {
                deferred.resolve(data.Model);
            });
        }
        return deferred.promise();
    };

    // OrganisationController Public API
    // ---------------------------------

    // Show organisation activity
    OrganisationController.showOrganisationStream = function (id) {
        log('showing organisation home stream', this, this);
        $.when(getModel(id))
            .done(function (model) {
                var organisation = new Organisation(model.Organisation);
                log('HACK: injected organisations/ into project id value');
                organisation.set('Id', 'organisations/' + id);
                var organisationLayoutView = new OrganisationLayoutView({ model: organisation });
                app.showFormContentView(organisationLayoutView, 'organisations');
                if (app.isPrerendering('organisations')) {
                    organisationLayoutView.showBootstrappedDetails();
                }
                organisationLayoutView.showStream();
                app.setPrerenderComplete();
            });
    };

    // Show an organisation
    OrganisationController.showOrganisationForm = function (id) {
        log('organisationController:showOrganisationForm');
        $.when(getModel(id))
            .done(function (model) {
                log(model);
                var organisation = new Organisation(model.Organisation);
                var organisationFormLayoutView = new OrganisationFormLayoutView({ model: organisation });
                app.showFormContentView(organisationFormLayoutView, 'organisations');
                if (app.isPrerendering('organisations')) {
                    organisationFormLayoutView.showBootstrappedDetails();
                }
                app.setPrerenderComplete();
            });
    };

    // Show an organisation explore
    OrganisationController.showOrganisationExplorer = function () {
        log('organisationController:showOrganisations');
        $.when(getExploreList())
            .done(function (model) {
                OrganisationController.organisationCollection = new OrganisationCollection(model.Organisations.PagedListItems);
                var organisationCollectionView = new OrganisationCollectionView({ collection: OrganisationController.organisationCollection });
                app.showFormContentView(organisationCollectionView, 'organisations');
                //app.content[app.getShowViewMethodName('projects')](projectCollectionView);
                if (app.isPrerendering('organisations')) {
                    organisationCollectionView.showBootstrappedDetails();
                }
                app.setPrerenderComplete();
            });
    };

    app.addInitializer(function () {
        log('FIRING ORGANISATION ROUTER');
        this.organisationRouter = new OrganisationRouter({
            controller: OrganisationController
        });
        log('ORGANISATION ROUTER - DONE');
    });

    return OrganisationController;
});