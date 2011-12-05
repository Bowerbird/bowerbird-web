// inherits from FormController

function ObservationUpdateFormController() {
    FormController.call();
}

ObservationUpdateFormController.prototype = new FormController();
ObservationUpdateFormController.prototype.constructor = ObservationUpdateFormController;
ObservationUpdateFormController.prototype.baseClass = FormController.prototype.constructor;