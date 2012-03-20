
window.Bowerbird.Views.ObservationCreateFormView = Backbone.View.extend({

    tagName: 'section',

    className: 'form single-medium',

    events: {
        'click #cancel': '_cancel',
        'click #save': '_save',
        "change input#title": "_contentChanged",
        "change input#observedOn": "_contentChanged"
    },

    template: $.template('observationCreateFormTemplate', $('#observation-create-form-template')),

    initialize: function (options) {
        _.extend(this, Backbone.Events);
        this.appView = options.appView;
        this.observation = options.observation;
        this.editMediaView = new Bowerbird.Views.EditMediaView({ el: $('#media-resources-fieldset'), observation: options.observation });
        this.editMapView = new Bowerbird.Views.EditMapView({ el: $('#location-fieldset'), observation: options.observation });
    },

    render: function () {
        $.tmpl('observationCreateFormTemplate', app.get('newObservation').toJSON()).appendTo(this.$el);
        window.scrollTo(0, 0);
        return this;
    },

    start: function () {
        this.editMediaView.render();
        this.editMapView.render();
    },

    _cancel: function () {
        app.set('newObservation', null);
        this.$el.remove();
        this.appView.showStreamView();
        app.router.navigate(app.stream.get('uri'), { trigger: true });
    },

    _contentChanged: function (e) {
        var target = $(e.currentTarget);
        var data = {};
        data[target.attr('name')] = target.attr('value');
        this.observation.set(data);
    },

    _save: function () {
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

    //    _onUploadAdd: function (e, data) {
    //        var self = this;
    //        $.each(data.files, function (index, file) {
    //            if (file != null) {
    //                self.currentUploadKey++;
    //                var mediaResource = new Bowerbird.Models.MediaResource({ key: self.currentUploadKey });
    //                self.observation.addMediaResources.add(mediaResource);
    //                var mediaResourceItemView = new Bowerbird.Views.MediaResourceItemView({ mediaResource: mediaResource });
    //                self.mediaResourceItemViews.push(mediaResourceItemView);
    //                $('.media-resource-items').append(mediaResourceItemView.render().el);

    //                loadImage(
    //                    data.files[0],
    //                    function (img) {
    //                        if (img instanceof HTMLImageElement) { // FF seems to fire this handler twice, on second time returning error, which we ignore :(
    //                            mediaResourceItemView.showTempMedia(img);
    //                        }
    //                    },
    //                    {
    //                        maxHeight: 220
    //                    }
    //                );
    //            }
    //        });
    //        data.submit();
    //    },

    //    _onSubmitUpload: function (e, data) {
    //        data.formData = { key: this.currentUploadKey, originalFileName: data.files[0].name };
    //    },

    //    _onUploadDone: function (e, data) {
    //        var mediaResource = _.find(this.observation.allMediaResources(), function (item) {
    //            return item.get('key') == data.result.key;
    //        });
    //        mediaResource.set(data.result);
    //    },

    //    _initMediaUploader: function () {
    //        $('#fileupload').fileupload({
    //            dataType: 'json',
    //            paramName: 'file',
    //            url: '/members/mediaresource/observationupload',
    //            add: this._onUploadAdd,
    //            submit: this._onSubmitUpload,
    //            done: this._onUploadDone
    //        });
    //    }
});

