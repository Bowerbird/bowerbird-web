/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// OrganisationController & OrganisationRouter
// ---------------------------------
define(['jquery', 'underscore', 'backbone', 'app', 'models/organisation', 'collections/organisationcollection', 'collections/activitycollection', 'collections/sightingcollection',
        'collections/usercollection', 'views/organisationdetailsview', 'views/organisationformview', 'views/organisationexploreview'],
function ($, _, Backbone, app, Organisation, OrganisationCollection, ActivityCollection, SightingCollection, UserCollection, OrganisationDetailsView, OrganisationFormView, OrganisationExploreView) {

    var OrganisationRouter = Backbone.Marionette.AppRouter.extend({
        appRoutes: {
            //'organisations': 'showExplore',
            'organisations/create*': 'showCreateForm'
            //'organisations/:id/posts*': 'showPosts',
            //'organisations/:id/members*': 'showMembers',
            //'organisations/:id/about': 'showAbout',
            //'organisations/:id/update': 'showUpdateForm',
            //'organisations/:id': 'showOrganisationDetails'
        }
    });

    var OrganisationController = {};

    var getModel = function (uri, action) {
        var deferred = new $.Deferred();
        if (app.isPrerenderingView('organisations')) {
            deferred.resolve(app.prerenderedView.data);
        } else {
            $.ajax({
                url: uri,
                type: action || 'GET'
            }).done(function (data) {
                deferred.resolve(data.Model);
            });
        }
        return deferred.promise();
    };

    // Show organisation form
    var showOrganisationForm = function (uri) {
        $.when(getModel(uri))
            .done(function (model) {
                var organisation = new Organisation(model.Organisation);

                var options = { model: organisation, teams: model.Teams };

                if (app.isPrerenderingView('organisations')) {
                    options['el'] = '.organisation-form';
                }

                var organisationFormView = new OrganisationFormView(options);
                app.showContentView('Edit Organisation', organisationFormView, 'organisations');
            });
    };

    // Public API
    // ----------

    // Show organisation details
    OrganisationController.showOrganisationDetails = function (id) {
        // Beacause IE is using has fragments, we have to fix the id manually for IE
        var url = id;
        if (url.indexOf('organisations') == -1) {
            url = '/organisations/' + url;
        }

        $.when(getModel(url))
            .done(function (model) {
                var organisation = new Organisation(model.Organisation);
                var activityCollection = new ActivityCollection(model.Activities.PagedListItems, { id: organisation.id });
                activityCollection.setPageInfo(model.Activities);

                if (app.content.currentView instanceof OrganisationDetailsView && app.content.currentView.model.id === organisation.id) {
                    app.content.currentView.showActivity(activityCollection);
                } else {
                    var options = { model: organisation };
                    if (app.isPrerenderingView('organisations')) {
                        options['el'] = '.organisation';
                    }
                    var organisationDetailsView = new OrganisationDetailsView(options);

                    app.showContentView(organisation.get('Name'), organisationDetailsView, 'organisations', function () {
                        organisationDetailsView.showActivity(activityCollection);
                    });
                }
            });
    };

    OrganisationController.showPosts = function (id, params) {
        $.when(getModel('/organisations/' + id + '/posts?sort=' + (params && params.sort ? params.sort : 'latestadded')))
            .done(function (model) {
                var organisation = new Organisation(model.Organisation);
                var sightingCollection = new SightingCollection(model.Sightings.PagedListItems, { organisationId: organisation.id, page: model.Query.page, pageSize: model.Query.PageSize, total: model.Sightings.TotalResultCount,
                    viewType: model.Query.View, sortBy: model.Query.Sort
                });

                if (app.content.currentView instanceof OrganisationDetailsView && app.content.currentView.model.id === organisation.id) {
                    app.content.currentView.showSightings(sightingCollection);
                } else {
                    var options = { model: organisation };
                    if (app.isPrerenderingView('organisations')) {
                        options['el'] = '.organisation';
                    }
                    var organisationDetailsView = new OrganisationDetailsView(options);

                    app.showContentView(organisation.get('Name'), organisationDetailsView, 'organisations', function () {
                        organisationDetailsView.showSightings(sightingCollection);
                    });
                }
            });
    };

    OrganisationController.showMembers = function (id, params) {
        $.when(getModel('/organisations/' + id + '/members?sort=' + (params && params.sort ? params.sort : 'a-z')))
        .done(function (model) {
            var organisation = new Organisation(model.Organisation);
            var userCollection = new UserCollection(model.Members.PagedListItems, { organisationId: organisation.id, page: model.Query.page, pageSize: model.Query.PageSize, total: model.Members.TotalResultCount, viewType: model.Query.View, sortBy: model.Query.Sort });

            if (app.content.currentView instanceof OrganisationDetailsView && app.content.currentView.model.id === organisation.id) {
                app.content.currentView.showMembers(userCollection);
            } else {
                var options = { model: organisation };
                if (app.isPrerenderingView('organisations')) {
                    options['el'] = '.organisation';
                }
                var organisationDetailsView = new OrganisationDetailsView(options);

                app.showContentView(organisation.get('Name'), organisationDetailsView, 'organisations', function () {
                    organisationDetailsView.showMembers(userCollection);
                });
            }
        });
    };

    OrganisationController.showAbout = function (id) {
        $.when(getModel('/organisations/' + id + '/about'))
        .done(function (model) {
            var organisation = new Organisation(model.Organisation);

            if (app.content.currentView instanceof OrganisationDetailsView && app.content.currentView.model.id === organisation.id) {
                app.content.currentView.showAbout(model.OrganisationAdministrators, model.ActivityTimeseries);
            } else {
                var options = { model: organisation };
                if (app.isPrerenderingView('organisations')) {
                    options['el'] = '.organisation';
                }
                var organisationDetailsView = new OrganisationDetailsView(options);

                app.showContentView(organisation.get('Name'), organisationDetailsView, 'organisations', function () {
                    organisationDetailsView.showAbout(model.OrganisationAdministrators, model.ActivityTimeseries);
                });
            }
        });
    };

    // Show organisation create form
    OrganisationController.showCreateForm = function (id) {
        var uri = '/organisations/create';
        if (id) {
            uri += id;
        }
        showOrganisationForm(uri);
    };

    // Show organisation update form
    OrganisationController.showUpdateForm = function (id) {
        showOrganisationForm('/organisations/' + id + '/update');
    };

    // Show organisation explore
    OrganisationController.showExplore = function (params) {
        $.when(getModel('/organisations?sort=' + (params && params.sort ? params.sort : 'newest')))
        .done(function (model) {
            var organisationCollection = new OrganisationCollection(model.Organisations.PagedListItems, { page: model.Query.page, pageSize: model.Query.PageSize, total: model.Organisations.TotalResultCount, sortBy: model.Query.Sort });

            var options = { collection: organisationCollection };

            if (app.authenticatedUser) {
                options.model = app.authenticatedUser.user;
            }

            if (app.isPrerenderingView('organisations')) {
                options['el'] = '.organisations';
            }
            var organisationExploreView = new OrganisationExploreView(options);

            app.showContentView('Organisations', organisationExploreView, 'organisations', function () {
            });
        });
    };

    // Event Handlers
    // --------------

    app.vent.on('joinOrganisation', function (organisation) {
        $.when(getModel('/' + organisation.id + '/join', 'POST'));
    });

    app.vent.on('leaveOrganisation', function (organisation) {
        $.when(getModel('/' + organisation.id + '/leave', 'POST'))
            .done(function (model) {
                app.authenticatedUser.organisations.remove(organisation.id);
            });
    });

    app.vent.on('organisationAdded:', function (organisation) {
        if (OrganisationController.organisationCollection) {
            OrganisationController.organisationCollection.add(organisation);
        }
    });

    app.addInitializer(function () {
        this.organisationRouter = new OrganisationRouter({
            controller: OrganisationController
        });
    });

    return OrganisationController;
});