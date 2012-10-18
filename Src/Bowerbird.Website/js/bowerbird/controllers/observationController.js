/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />
/// <reference path="../models/observation.js" />

// ObservationController & ObservationRouter
// -----------------------------------------

define(['jquery', 'underscore', 'backbone', 'app', 'models/observation', 'views/observationdetailsview', 'views/observationformview'],
function ($, _, Backbone, app, Observation, ObservationDetailsView, ObservationFormView) {
    var ObservationRouter = Backbone.Marionette.AppRouter.extend({
        appRoutes: {
            'observations/create*': 'showObservationCreateForm',
            'observations/:id/update': 'showObservationUpdateForm',
            'observations/:id': 'showObservationDetails'
        }
    });

    var ObservationController = {};

    var showObservationForm = function (uri) {
        $.when(getModel(uri))
            .done(function (model) {
                var observation = new Observation(model.Observation);

                var options = { model: observation, categorySelectList: model.CategorySelectList, categories: model.Categories, projectsSelectList: model.ProjectsSelectList };

                if (app.isPrerendering('observations')) {
                    options['el'] = '.observation-form';
                }

                var observationFormView = new ObservationFormView(options);
                app.showContentView('Edit Observation', observationFormView, 'observations');
            });
    };

    var getModel = function (uri, action) {
        var deferred = new $.Deferred();
        if (app.isPrerendering('observations')) {
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

    // Public API
    // ----------

    ObservationController.showObservationDetails = function (id) {
        $.when(getModel(id))
            .done(function (model) {
                var observation = new Observation(model.Observation);
                
                var options = { model: observation };

                if (app.isPrerendering('observations')) {
                    options['el'] = '.observation';
                }

                var observationDetailsView = new ObservationDetailsView(options);
                app.showContentView(observation.get('Title'), observationDetailsView, 'observations');
            });
    };

    ObservationController.showObservationCreateForm = function (id) {
        var uri = '/observations/create';

        if (id) {
            var uriParts = [];
            if (id.projectid) {
                uriParts.push('projectid=' + id.projectid);
            }
            if (id.category) {
                uriParts.push('category=' + id.category);
            }
            uri += '?' + uriParts.join('&');
        }

        showObservationForm(uri);
    };

    ObservationController.showObservationUpdateForm = function (id) {
        showObservationForm('/observations/' + id + '/update');
    };

    ObservationController.mediaResourceUploaded = function (e, mediaResource) {
        app.vent.trigger('mediaResourceUploaded:', mediaResource);
    };

    // Event Handlers
    // --------------

    app.addInitializer(function () {
        this.observationRouter = new ObservationRouter({
            controller: ObservationController
        });
    });

    return ObservationController;
});