/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />
/// <reference path="../models/observation.js" />

// RecordController & RecordRouter
// -------------------------------

define(['jquery', 'underscore', 'backbone', 'app', 'views/recordformview', 'models/record'],
function ($, _, Backbone, app, RecordFormView, Record) {

    var RecordRouter = Backbone.Marionette.AppRouter.extend({
        appRoutes: {
            'records/create': 'showRecordForm',
            'records/:id/update': 'showRecordForm'
            //'records/:id': 'showRecordDetails'
        }
    });

    var RecordController = {};

    //    // Helper method to load project layout, taking into account bootstrapped data and prerendered view
    //    var showObservationLayoutView = function (observation) {
    //        var observationLayoutView = new ObservationLayoutView({ model: observation });
    //        app.showFormContentView(observationLayoutView, 'records');

    //        if (app.isPrerenderingView('records')) {
    //            observationLayoutView.showBootstrappedDetails();
    //        }

    //        return observationLayoutView;
    //    };

    var getModel = function (id, projectId) {
        var url = '/records/create';
        if (projectId) {
            url += '?id=' + projectId;
        }
        if (id) {
            url = id;
        }
        var deferred = new $.Deferred();
        if (app.isPrerenderingView('records')) {
            deferred.resolve(app.prerenderedView.data);
        } else {
            $.ajax({
                url: url
            }).done(function (data) {
                deferred.resolve(data.Model);
            });
        }
        return deferred.promise();
    };

    // Public API
    // ----------

    //    RecordController.showRecordDetails = function (id) {
    //        $.when(getModel(id))
    //            .done(function (model) {
    //                var record = new Record(model.Record);
    //                app.updateTitle(record.get('Title'));
    //                var recordLayoutView = showRecordLayoutView(record);
    //                recordLayoutView.showRecordDetails(record);
    //                app.setPrerenderComplete();
    //            });
    //    };

    RecordController.showRecordForm = function (id) {
        $.when(getModel(id))
            .done(function (model) {
                var record = new Record(model.Record);
                if (record.id) {
                    app.updateTitle('Edit Record');
                } else {
                    app.updateTitle('New Record');
                }

                var recordFormView = new RecordFormView({ model: record, categories: model.Categories });
                app.showFormContentView(recordFormView, 'records');

                if (app.isPrerenderingView('records')) {
                    recordFormView.showBootstrappedDetails();
                }

                //observationLayoutView.showObservationForm(observation, model.Categories);
                app.setPrerenderComplete();
            });
    };

    app.addInitializer(function () {
        this.recordRouter = new RecordRouter({
            controller: RecordController
        });
    });

    return RecordController;
});