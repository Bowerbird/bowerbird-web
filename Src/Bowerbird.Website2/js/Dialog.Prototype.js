//function Dialog(dialogId, templateId, model, callbackOnOk) {
//    
//    this.currentDialogId = dialogId;
//    this.templateId = templateId;
//    this.model = model;
//    this.callbackOnOk = callbackOnOk;

//    this.make(dialogId, templateId, model, callbackOnOk);
//}

//Dialog.prototype = {

//    currentDialogId: '',
//    templateId: '',
//    model: '',
//    callbackOnOk: '',

//    make: function (dialogId, templateId, model, callbackOnOk) {
//        
//        $('body').append('<div id=' + dialog.currentDialogId + '></div>');

//        $('#' + templateId).tmpl().appendTo('#' + dialog.currentDialogId);

//        $('#' + this.currentDialogId).dialog({
//            autoOpen: false,
//            width: 600,
//            buttons: [
//                {
//                    text: "Ok",
//                    click: function (event) {
//                        this.callbackOnOk(event);
//                    }
//                },
//                {
//                    text: "Cancel",
//                    click: function () {
//                        $(this).dialog("close");
//                        $('#' + this.currentDialogId).remove();
//                        this.currentDialogId = '';
//                    }
//                }
//            ],
//            modal: true
//        });

//        $('#' + this.currentDialogId).bind("dialogclose", function (event, ui) {
//            $('#' + this.currentDialogId).remove();
//            this.currentDialogId = '';
//        });
//    },

//    remove: function () {
//        $('#' + this.currentDialogId).dialog("close");
//        $('#' + this.currentDialogId).remove();
//        this.currentDialogId = '';
//    },

//    show: function () {
//        $('#' + this.currentDialogId).dialog('open');
//    }
//};