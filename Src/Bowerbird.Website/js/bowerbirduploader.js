var uploader;
var mediaResources = new Array();

function buildMediaUploader(recordType, multiuploads) {
    uploader = new qq.FileUploader({
        element: document.getElementById('media-uploader'),
        action: '/members/mediaresource/'+recordType+'upload',

        template: '<div class="media-resource-items" id="media-resource-items"><ul class="qq-upload-list" style="display: none;"></ul><div class="media-resource-dropzone">Drop Files Here</div></div><div class="field"><div>' +
                    '<div id="media-resource-upload-button" class="button media-resource-upload-button">Choose Files</div>' +
                    '<input type="button" value="Import From Website" id="media-resource-import-button"/></div></div>',
        fileTemplate: '<li>' +
                    '<span class="qq-upload-file"></span>' +
                    '<span class="qq-upload-spinner"></span>' +
                    '<span class="qq-upload-size"></span>' +
                    '<a class="qq-upload-cancel" href="#">Cancel</a>' +
                '</li>',

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

        multiple: multiuploads,
        debug: true,
        onComplete: function (id, fileName, responseText) {
            $('#media-resource-items').append('<div class="media-resource-uploaded"><img src="' + responseText.imageUrl + '" width="200px" /><div><span>' + responseText.fileName + '</span><span>' + responseText.fileSize + '<span></div></div>');
            mediaResources.push(responseText.Id);
        }
    });
}

function buildAvatarUploader() {
    uploader = new qq.FileUploader({
        element: document.getElementById('media-uploader'),
        action: '/members/mediaresource/avatarupload',

        template: '<div class="media-resource-items" id="media-resource-items"><ul class="qq-upload-list" style="display: none;"></ul><div class="media-resource-dropzone">Drop Files Here</div></div><div class="field"><div>' +
                    '<div id="media-resource-upload-button" class="button media-resource-upload-button">Choose File</div>' +
                    '<input type="button" value="Import From Website" id="media-resource-import-button"/></div></div>',
        fileTemplate: '<li>' +
                    '<span class="qq-upload-file"></span>' +
                    '<span class="qq-upload-spinner"></span>' +
                    '<span class="qq-upload-size"></span>' +
                    '<a class="qq-upload-cancel" href="#">Cancel</a>' +
                '</li>',

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

        multiple: false,
        debug: true,
        onComplete: function (id, fileName, responseText) {
            $('#media-resource-items').empty();
            $('#media-resource-items').append('<div class="media-resource-uploaded"><img src="' + responseText.imageUrl + '" width="200px" /><div><span>' + responseText.fileName + '</span><span>' + responseText.fileSize + '<span></div></div>');
            $('#avatarId').val(responseText.Id);
        }
    });
}