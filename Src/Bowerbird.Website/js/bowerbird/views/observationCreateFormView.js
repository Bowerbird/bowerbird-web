
window.Bowerbird.Views.ObservationCreateFormView = Backbone.View.extend({

    tagName: 'section',

    className: 'form single-medium',

    events: {
        "click #cancel": "cancel",
        "click #save": "save"
    },

    template: $.template('observationCreateFormTemplate', $('#observation-create-form-template')),

    initialize: function (options) {
        _.extend(this, Backbone.Events);
        _.bindAll(this, 'start');
        this.appView = options.appView;
        this.uploader = null;
        this.mediaResources = new Array();
    },

    render: function () {
        $.tmpl('observationCreateFormTemplate', app.get('newObservation').toJSON()).appendTo(this.$el);
        window.scrollTo(0, 0);
        return this;
    },

    start: function () {
        buildMap();
        buildMediaUploader('observation', true);
    },

    cancel: function () {
        app.set('newObservation', null);
        this.$el.remove();
        this.appView.showStreamView();
        app.router.navigate(app.stream.get('uri'), { trigger: true });
    },

    save: function () {
        alert('not yet!');
        //this.model.set({
        //    "title": $("#title").attr("value"),
        //    "address": $("#address").attr("value"),
        //    "latitude": $("#latitude").attr("value"),
        //    "longitude": $("#longitude").attr("value"),
        //    "observedOn": $("#observedOn").attr("value"),
        //    "isIdentificationRequired": $("#isIdentificationRequired").attr("value"),
        //    "observationCategory": $("#observationCategory").attr("value")
        //});

        ////window.observations.add(this.model);

        //this.model.save();

        //this.hide($(this.el));
    }

});