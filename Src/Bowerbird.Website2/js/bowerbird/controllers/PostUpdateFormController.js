// inherits from FormController

function PostUpdateFormController() {
    FormController.call();
}

PostUpdateFormController.prototype = new FormController();
PostUpdateFormController.prototype.constructor = PostUpdateFormController;
PostUpdateFormController.prototype.baseClass = FormController.prototype.constructor;