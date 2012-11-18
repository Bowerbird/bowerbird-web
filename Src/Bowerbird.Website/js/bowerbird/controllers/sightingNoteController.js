/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />
/// <reference path="../models/observation.js" />

// SightingNoteController & SightingNoteRouter
// -------------------------------------------

define(['jquery', 'underscore', 'backbone', 'app', 'models/sightingnote', 'models/observation', 'views/sightingnoteformview'],
function ($, _, Backbone, app, SightingNote, Sighting, SightingNoteFormView) {

    var SightingNoteRouter = Backbone.Marionette.AppRouter.extend({
        appRoutes: {
            'sightingnotes/create*': 'showSightingNoteCreateForm',
            'sightingnotes/:id/update': 'showSightingNoteUpdateForm'
        }
    });

    var SightingNoteController = {};

    var showSightingNoteForm = function (uri) {
        $.when(getModel(uri))
            .done(function (model) {
                var sightingNote = new SightingNote(model.SightingNote);

                var options = { model: sightingNote, sighting: new Sighting(model.Sighting), descriptionTypesSelectList: model.DescriptionTypesSelectList };

                if (app.isPrerenderingView('sightingnotes')) {
                    options['el'] = '.sighting-note-form';
                }

                var sightingNoteFormView = new SightingNoteFormView(options);
                app.showContentView('Edit Sighting Note', sightingNoteFormView, 'sightingnotes');
            });
    };

    var getModel = function (uri, action) {
        var deferred = new $.Deferred();
        if (app.isPrerenderingView('sightingnotes')) {
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

    SightingNoteController.showSightingNoteCreateForm = function (id) {
        var uri = '/sightingnotes/create';

        if (id) {
            var uriParts = [];
            if (id.id) {
                uriParts.push('id=' + id.id);
            }
            uri += '?' + uriParts.join('&');
        }

        //uri += '?id=' + id;

        showSightingNoteForm(uri);
    };

    SightingNoteController.showSightingNoteUpdateForm = function (id) {
        showSightingNoteForm('/sightingnotes/' + id + '/update');
    };

    // Event Handlers
    // --------------

    app.addInitializer(function () {
        this.sightingNoteRouter = new SightingNoteRouter({
            controller: SightingNoteController
        });
    });

    return SightingNoteController;
});