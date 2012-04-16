
window.Bowerbird.Views.EditMediaView = Backbone.View.extend({
    
    Id: 'media-resources-fieldset',

    initialize: function (options) {
        _.extend(this, Backbone.Events);
        _.bindAll(this,
        'render',
        '_initMediaUploader',
        '_onUploadDone',
        '_onSubmitUpload',
        '_onUploadAdd'
        );
        this.mediaResourceItemViews = [];
        this.Observation = options.Observation;
        this.currentUploadKey = 0;
    },

    render: function () {
        this._initMediaUploader();
        return this;
    },

    _initMediaUploader: function () {
        $('#fileupload').fileupload({
            dataType: 'json',
            paramName: 'file',
            url: '/mediaresource/observationupload',
            add: this._onUploadAdd,
            submit: this._onSubmitUpload,
            done: this._onUploadDone
        });
    },

    _onUploadAdd: function (e, data) {
        var self = this;
        $.each(data.files, function (index, file) {
            if (file != null) {
                self.currentUploadKey++;
                var mediaResource = new Bowerbird.Models.MediaResource({ Key: self.currentUploadKey });
                self.Observation.addMediaResources.add(mediaResource);
                var mediaResourceItemView = new Bowerbird.Views.MediaResourceItemView({ MediaResource: mediaResource });
                self.mediaResourceItemViews.push(mediaResourceItemView);
                $('#media-resource-add-pane').before(mediaResourceItemView.render().el);
                loadImage(
                    data.files[0],
                    function (img) {
                        if (img instanceof HTMLImageElement) { // FF seems to fire this handler twice, on second time returning error, which we ignore :(
                            mediaResourceItemView.showTempMedia(img);
                            $('#media-resource-items').animate({ scrollLeft: 100000 });
                        }
                    },
                    {
                        maxHeight: 220
                    }
                );
            }
        });
        data.submit();
    },

    _onSubmitUpload: function (e, data) {
        data.formData = { Key: this.currentUploadKey, OriginalFileName: data.files[0].name };
    },

    _onUploadDone: function (e, data) {
        var mediaResource = _.find(this.Observation.allMediaResources(), function (item) {
            return item.get('Key') == data.result.key;
        });
        mediaResource.set(data.result);
        //$('#media-resource-items').animate({ scrollLeft: 100000 });
    }
});

//function buildAvatarUploader() {
//    uploader = new qq.FileUploader({
//        element: document.getElementById('media-uploader'),
//        action: '/members/mediaresource/avatarupload',

//        template: '<div class="media-resource-items" id="media-resource-items"><ul class="qq-upload-list" style="display: none;"></ul><div class="media-resource-dropzone">Drop Files Here</div></div><div class="field"><div>' +
//                    '<div id="media-resource-upload-button" class="button media-resource-upload-button">Choose File</div>' +
//                    '<input type="button" value="Import From Website" id="media-resource-import-button"/></div></div>',
//        fileTemplate: '<li>' +
//                    '<span class="qq-upload-file"></span>' +
//                    '<span class="qq-upload-spinner"></span>' +
//                    '<span class="qq-upload-size"></span>' +
//                    '<a class="qq-upload-cancel" href="#">Cancel</a>' +
//                '</li>',

//        classes: {
//            // used to get elements from templates
//            button: 'media-resource-upload-button', //'qq-upload-button',
//            drop: 'media-resource-dropzone', //'qq-upload-drop-area',
//            dropActive: 'qq-upload-drop-area-active',
//            list: 'qq-upload-list',

//            file: 'qq-upload-file',
//            spinner: 'qq-upload-spinner',
//            size: 'qq-upload-size',
//            cancel: 'qq-upload-cancel',

//            // added to list item when upload completes
//            // used in css to hide progress spinner
//            success: 'qq-upload-success',
//            fail: 'qq-upload-fail'
//        },

//        multiple: false,
//        debug: true,
//        onComplete: function (id, fileName, responseText) {
//            $('#media-resource-items').empty();
//            $('#media-resource-items').append('<div class="media-resource-uploaded"><img src="' + responseText.imageUrl + '" width="200px" /><div><span>' + responseText.fileName + '</span><span>' + responseText.fileSize + '<span></div></div>');
//            $('#AvatarId').val(responseText.Id);
//        }
//    });
//}