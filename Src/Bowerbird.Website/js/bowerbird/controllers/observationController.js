/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />
/// <reference path="../models/observation.js" />

// ObservationController & ObservationRouter
// -----------------------------------------

define(['jquery', 'underscore', 'backbone', 'app', 'models/observation', 'models/sighting', 'models/sightingnote', 'views/observationdetailsview', 'views/observationformview', 'views/sightingnoteformview'],
function ($, _, Backbone, app, Observation, Sighting, SightingNote, ObservationDetailsView, ObservationFormView, SightingNoteFormView) {
    var ObservationRouter = Backbone.Marionette.AppRouter.extend({
        appRoutes: {
            'observations/:id/createnote': 'showSightingNoteCreateForm',
            'observations/:sightingId/updatenote/:sightingNoteId': 'showSightingNoteUpdateForm',
            'observations/create*': 'showObservationCreateForm',
            'observations/:id/update': 'showObservationUpdateForm',
            'observations/:id': 'showObservationDetails'
        }
    });

    var ObservationController = {};

    var showObservationForm = function (uri) {
        $.when(getModel(uri, 'observations'))
            .done(function (model) {
                var observation = new Observation(model.Observation);

                var options = { model: observation, categorySelectList: model.CategorySelectList, categories: model.Categories, projectsSelectList: model.ProjectsSelectList };

                if (app.isPrerenderingView('observations')) {
                    options['el'] = '.observation-form';
                }

                var observationFormView = new ObservationFormView(options);
                app.showContentView('Edit Sighting', observationFormView, 'observations');
            });
    };

    var showSightingNoteForm = function (uri) {
        $.when(getModel(uri, 'sightingnotes'))
        .done(function (model) {
            var sightingNote = new SightingNote(model.SightingNote);

            var options = { model: sightingNote, sighting: new Sighting(model.Sighting), descriptionTypesSelectList: model.DescriptionTypesSelectList, categories: model.Categories, categorySelectList: model.CategorySelectList };

            if (app.isPrerenderingView('sightingnotes')) {
                options['el'] = '.sighting-note-form';
            }

            var sightingNoteFormView = new SightingNoteFormView(options);
            app.showContentView('Edit Sighting Note', sightingNoteFormView, 'sightingnotes');
        });
    };

    var getModel = function (uri, viewName) {
        var deferred = new $.Deferred();
        if (app.isPrerenderingView(viewName)) {
            deferred.resolve(app.prerenderedView.data);
        } else {
            $.ajax({
                url: uri
                //type: action
            }).done(function (data) {
                deferred.resolve(data.Model);
            });
        }
        return deferred.promise();
    };

    // Public API
    // ----------

    ObservationController.showObservationDetails = function (id) {
        $.when(getModel(id, 'observations'))
            .done(function (model) {
                var observation = new Sighting(model.Observation);
                
                var options = { model: observation };

                if (app.isPrerenderingView('observations')) {
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

    ObservationController.showSightingNoteCreateForm = function (id) {
        showSightingNoteForm('/observations/' + id + '/createnote');
    };

    ObservationController.showSightingNoteUpdateForm = function (sightingId, sightingNoteId) {
        showSightingNoteForm('/observations/' + sightingId + '/updatenote/' + sightingNoteId);
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