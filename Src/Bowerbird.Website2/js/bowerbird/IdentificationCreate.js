var identificationCreate = {
    init: function () {
    },

    makeForm: function () {
        dialog.make(
            'identificationCreateDialog',
            'identificationCreateTemplate',
            null,
            function (event) {
                if ($('#identificationCreateDialog form').valid()) {
                    $.ajax({
                        url: '/identification/create', //$('#observationCreateDialog form').action,
                        type: 'post', //$('#observationCreateDialog form').method,
                        contentType: 'application/json; charset=utf-8',
                        data: JSON.stringify({
                            'ObservationKey': $('#ObservationKey').val(),
                            'CommonName': $('#CommonName').val(),
                            'ScientificName': $('#ScientificName').val(),
                            'Taxonomy': $('#Taxonomy').val()
                        }),
                        success: function (result) {
                            $('#validation-result').empty();
                            $('#validation-result').append('<span>' + result.Msg + '</span>');

                            dialog.remove();
                        }
                    });
                }
            });

        // Hookup knockout events
        dialog.show();
    }
};