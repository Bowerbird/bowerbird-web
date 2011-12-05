// inherits from FormController

function IdentificationUpdateFormController() {
    FormController.call();
}

IdentificationUpdateFormController.prototype = new FormController();
IdentificationUpdateFormController.prototype.constructor = IdentificationUpdateFormController;
IdentificationUpdateFormController.prototype.baseClass = FormController.prototype.constructor;