var TriageEnquiryUserImageAdd = {
    SiteUrl: '',
    init: function (siteUrl) {
        TriageEnquiryUserImageAdd.SiteUrl = siteUrl;
        createUploader();
        $('#image-details-form').hide();
        $('#file-uploader').show();
    }
};

function createUploader() {
    $('#file-uploader').show();
    $('#image-details-form').hide();
    var uploader = new qq.FileUploader({
        element: document.getElementById('file-uploader'),
        action: 'Upload',
        multiple: false,
        debug: true,
        onComplete: function (id, fileName, responseText) {
            var uploadedImage = $('#file-uploaded');
            uploadedImage.append('<img src="' + responseText.FileName + '" width="240px" />');
            $('#file-uploader').hide();
            $('input[name=TempImageId]').val(responseText.TempFileId);
            $('#TempImageId').val(responseText.TempFileId);
            $('#image-details-form').show();
            initialize();
        }
    });
}

function ClearForm() {
    createUploader();
}

function showLocationPanel() {
    $('#location-panel').fadeIn();
    $('#image-details-form').show();
    google.maps.event.trigger(map, 'resize'); // one of these ought to work
    initialize(); // one of these ought to work
    moveToMarker();
}