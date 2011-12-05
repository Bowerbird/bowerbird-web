// inherits from FormController

function CommentUpdateFormController() {
    FormController.call();
}

CommentUpdateFormController.prototype = new FormController();
CommentUpdateFormController.prototype.constructor = CommentUpdateFormController;