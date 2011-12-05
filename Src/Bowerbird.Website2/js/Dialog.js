//var dialog = {
//    currentDialogId: '',

//    make: function (dialogId, templateId, model, callbackOnOk) {
//        dialog.currentDialogId = dialogId;
//        $('body').append('<div id=' + dialog.currentDialogId + '></div>');

//        $('#' + templateId).tmpl().appendTo('#' + dialog.currentDialogId);
//        
//        $('#' + dialog.currentDialogId).dialog({
//            autoOpen: false,
//            width: 600,
//            buttons: [
//                {
//                    text: "Ok",
//                    click: function (event) {
//                        callbackOnOk(event);
//                    }
//                },
//                {
//                    text: "Cancel",
//                    click: function () {
//                        $(this).dialog("close");
//                        $('#' + dialog.currentDialogId).remove();
//                        dialog.currentDialogId = '';
//                    }
//                }
//            ],
//            modal: true
//        });

//        $('#' + dialog.currentDialogId).bind( "dialogclose", function(event, ui) {
//            $('#' + dialogId).remove();
//            dialog.currentDialogId = '';
//        });
//    },

//    remove: function () {
//        $('#' + dialog.currentDialogId).dialog("close");
//        $('#' + dialogId).remove();
//        dialog.currentDialogId = '';
//    },

//    show: function () {
//        $('#' + dialog.currentDialogId).dialog('open');
//    }
//};
