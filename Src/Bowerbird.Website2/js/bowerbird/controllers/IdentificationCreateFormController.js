// inherits from FormController

function IdentificationCreateFormController() {
    FormController.call();
}

IdentificationCreateFormController.prototype = new FormController();
IdentificationCreateFormController.prototype.constructor = IdentificationCreateFormController;
IdentificationCreateFormController.prototype.baseClass = FormController.prototype.constructor;