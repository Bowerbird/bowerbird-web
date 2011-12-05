// inherits from FormController

function CommentCreateFormController() {
    FormController.call();
}

CommentCreateFormController.prototype = new FormController();
CommentCreateFormController.prototype.constructor = CommentCreateFormController;