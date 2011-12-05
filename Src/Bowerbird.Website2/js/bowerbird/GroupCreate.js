var groupCreate = {
    init: function () {
    },

    makeForm: function () {
        dialog.make(
            'groupCreateDialog',
            'groupCreateTemplate',
            null,
            function (event) {
                if ($('#groupCreateDialog form').valid()) {
                    $.ajax({
                        url: '/group/create', //$('#observationCreateDialog form').action,
                        type: 'post', //$('#observationCreateDialog form').method,
                        contentType: 'application/json; charset=utf-8',
                        data: JSON.stringify({
                            'Name': $('#Name').val(),
                            'MissionStatement': $('#MissionStatement').val(),
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