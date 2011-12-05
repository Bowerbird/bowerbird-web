// inherits from FormController

function ProjectUpdateFormController() {
    FormController.call();
}

ProjectUpdateFormController.prototype = new FormController();
ProjectUpdateFormController.prototype.constructor = ProjectUpdateFormController;
ProjectUpdateFormController.prototype.baseClass = FormController.prototype.constructor;