// inherits from FormController

function PostCreateFormController() {
    FormController.call();
}

PostCreateFormController.prototype = new FormController();
PostCreateFormController.prototype.constructor = PostCreateFormController;
PostCreateFormController.prototype.baseClass = FormController.prototype.constructor;