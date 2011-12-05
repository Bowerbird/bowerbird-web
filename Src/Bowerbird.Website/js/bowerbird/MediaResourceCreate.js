var mediaResourceCreate = {
    init: function () {
    },

    makeForm: function () {
        dialog.make(
            'mediaResourceCreateDialog',
            'mediaResourceCreateTemplate',
            null,
            function (event) {
                if ($('#mediaResourceCreateDialog form').valid()) {
                    $.ajax({
                        url: '/MediaResource/create', //$('#observationCreateDialog form').action,
                        type: 'post', //$('#observationCreateDialog form').method,
                        contentType: 'application/json; charset=utf-8',
                        data: JSON.stringify({
                            'Description': $('#Description').val()
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