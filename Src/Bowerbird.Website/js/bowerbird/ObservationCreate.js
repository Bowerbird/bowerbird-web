function ObservationCreate() {

    this.dialogId = 'observationCreateDialog';
    this.templateId = 'observationCreateTemplate';

    this.dialog = new Dialog(
        this.dialogId,
        this.templateId,
        null,
        this.callBackOnOk);

    this.dialog.show();
}

ObservationCreate.prototype = {

    dialog: '',
    dialogId: '',
    templateId: '',

    init: function () { },

    callBackOnOk: function (event) {
        if ($('#observationCreateDialog form').valid()) {
            $.ajax({
                url: '/observation/create', 
                type: 'post', 
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify({
                    'Description': $('#Description').val(),
                    'Address': $('#Address').val(),
                    'Latitude': $('#Latitude').val(),
                    'Longitude': $('#Longitude').val()
                }),
                success: function (result) {
                    $('#validation-result').empty();
                    $('#validation-result').append('<span>' + result.Msg + '</span>');

                    dialog.remove();
                }
            });
        }
    }

};