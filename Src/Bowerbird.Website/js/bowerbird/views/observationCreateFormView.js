
window.Bowerbird.Views.ObservationCreateFormView = Backbone.View.extend({

    tagName: 'section',

    className: 'form single-medium',

    events: {
        'click #cancel': 'cancel',
        'click #save': 'save'
    },

    template: $.template('observationCreateFormTemplate', $('#observation-create-form-template')),

    initialize: function (options) {
        _.extend(this, Backbone.Events);
        _.bindAll(this, '_onUploadComplete');
        this.appView = options.appView;
        this.mediaUploader = null;
        this.mediaResourceItemViews = [];
    },

    render: function () {
        $.tmpl('observationCreateFormTemplate', app.get('newObservation').toJSON()).appendTo(this.$el);
        window.scrollTo(0, 0);
        return this;
    },

    start: function () {
        buildMap();
        this._initMediaUploader();
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
    },

    _initMediaUploader: function () {
        this.mediaUploader = new qq.FileUploader({
            element: document.getElementById('media-resources-fieldset'),

            action: '/members/mediaresource/observationupload',

            template: $("<div />").append($.tmpl($('#observation-create-media-resource-uploader-template'))).html(),

            fileTemplate: $("<div />").append($.tmpl($('#observation-create-media-resource-file-template'))).html(),

            classes: {
                // used to get elements from templates
                button: 'media-resource-upload-button', //'qq-upload-button',
                drop: 'media-resource-dropzone', //'qq-upload-drop-area',
                dropActive: 'qq-upload-drop-area-active',
                list: 'qq-upload-list',

                file: 'qq-upload-file',
                spinner: 'qq-upload-spinner',
                size: 'qq-upload-size',
                cancel: 'qq-upload-cancel',

                // added to list item when upload completes
                // used in css to hide progress spinner
                success: 'qq-upload-success',
                fail: 'qq-upload-fail'
            },

            multiple: true,

            debug: true,

            onComplete: this._onUploadComplete
        });
    },

    _onUploadComplete: function (id, fileName, response) {
        //$('#observation-create-media-resource-uploaded-template').tmpl(response).appendTo('#media-resource-items');
        this._showMediaResource(response);
        //this.mediaResources.push(response.Id);
    },

    _showMediaResource: function (response) {
        var mediaResource = new Bowerbird.Models.MediaResource(response);
        var mediaResourceItemView = new Bowerbird.Views.MediaResourceItemView({ mediaResource: mediaResource });
        this.mediaResourceItemViews.push(mediaResourceItemView);
        $(".media-resource-items").append(mediaResourceItemView.render().el);
    }
});