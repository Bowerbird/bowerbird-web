var commentCreate = {
    init: function () {
    },

    makeForm: function () {
        dialog.make(
            'commentCreateDialog',
            'commentCreateTemplate',
            null,
            function (event) {
                if ($('#commentCreateDialog form').valid()) {
                    $.ajax({
                        url: '/comment/create', //$('#observationCreateDialog form').action,
                        type: 'post', //$('#observationCreateDialog form').method,
                        contentType: 'application/json; charset=utf-8',
                        data: JSON.stringify({
                            'ObservationKey': $('#ObservationKey').val(),
                            'Message': $('#Message').val()
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