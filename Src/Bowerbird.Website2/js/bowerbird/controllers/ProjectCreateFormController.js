// inherits from FormController

function ProjectCreateFormController() {
    FormController.call();
}

ProjectCreateFormController.prototype = new FormController();
ProjectCreateFormController.prototype.constructor = ProjectCreateFormController;
ProjectCreateFormController.prototype.baseClass = FormController.prototype.constructor;