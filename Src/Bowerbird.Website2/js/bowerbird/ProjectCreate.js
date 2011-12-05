var projectCreate = {
    init: function () {
    },

    makeForm: function () {
        dialog.make(
            'projectCreateDialog',
            'projectCreateTemplate',
            null,
            function (event) {
                if ($('#projectCreateDialog form').valid()) {
                    $.ajax({
                        url: '/project/create', //$('#observationCreateDialog form').action,
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