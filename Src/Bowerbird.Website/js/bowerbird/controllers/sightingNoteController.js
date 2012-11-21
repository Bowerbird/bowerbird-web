///// <reference path="../../libs/log.js" />
///// <reference path="../../libs/require/require.js" />
///// <reference path="../../libs/jquery/jquery-1.7.2.js" />
///// <reference path="../../libs/underscore/underscore.js" />
///// <reference path="../../libs/backbone/backbone.js" />
///// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />
///// <reference path="../models/observation.js" />

//// SightingNoteController & SightingNoteRouter
//// -------------------------------------------

//define(['jquery', 'underscore', 'backbone', 'app', 'models/sightingnote', 'models/sighting', 'views/sightingnoteformview'],
//function ($, _, Backbone, app, SightingNote, Sighting, SightingNoteFormView) {

//    var SightingNoteRouter = Backbone.Marionette.AppRouter.extend({
//        appRoutes: {
//            'observations/:id/createnote': 'showSightingNoteCreateForm',
//            'observations/:sightingId/updatenote/:sightingNoteId': 'showSightingNoteUpdateForm'
//        }
//    });

//    var SightingNoteController = {};

//    var showSightingNoteForm = function (uri) {
//        $.when(getModel(uri))
//            .done(function (model) {
//                var sightingNote = new SightingNote(model.SightingNote);

//                var options = { model: sightingNote, sighting: new Sighting(model.Sighting), descriptionTypesSelectList: model.DescriptionTypesSelectList, categories: model.Categories, categorySelectList: model.CategorySelectList };

//                if (app.isPrerenderingView('sightingnotes')) {
//                    options['el'] = '.sighting-note-form';
//                }

//                var sightingNoteFormView = new SightingNoteFormView(options);
//                app.showContentView('Edit Sighting Note', sightingNoteFormView, 'sightingnotes');
//            });
//    };

//    var getModel = function (uri, action) {
//        var deferred = new $.Deferred();
//        if (app.isPrerenderingView('sightingnotes')) {
//            deferred.resolve(app.prerenderedView.data);
//        } else {
//            $.ajax({
//                url: uri,
//                type: action || 'GET'
//            }).done(function (data) {
//                deferred.resolve(data.Model);
//            });
//        }
//        return deferred.promise();
//    };

//    // Public API
//    // ----------

//    SightingNoteController.showSightingNoteCreateForm = function (id) {
//        showSightingNoteForm('/observations/' + id + '/createnote');
//    };

//    SightingNoteController.showSightingNoteUpdateForm = function (sightingId, sightingNoteId) {
//        showSightingNoteForm('/observations/' + sightingId + '/updatenote/' + sightingNoteId);
//    };

//    // Event Handlers
//    // --------------

//    app.addInitializer(function () {
//        this.sightingNoteRouter = new SightingNoteRouter({
//            controller: SightingNoteController
//        });
//    });

//    return SightingNoteController;
//});