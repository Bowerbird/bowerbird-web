var postCreate = {
    init: function () {
    },

    makeForm: function () {
        dialog.make(
            'postCreateDialog',
            'postCreateTemplate',
            null,
            function (event) {
                if ($('#postCreateDialog form').valid()) {
                    $.ajax({
                        url: '/post/create', //$('#observationCreateDialog form').action,
                        type: 'post', //$('#observationCreateDialog form').method,
                        contentType: 'application/json; charset=utf-8',
                        data: JSON.stringify({
                            'Message': $('#Message').val(),
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