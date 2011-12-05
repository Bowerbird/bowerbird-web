// inherits from FormController

function GroupUpdateFormController() {
    FormController.call();
}

GroupUpdateFormController.prototype = new FormController();
GroupUpdateFormController.prototype.constructor = GroupUpdateFormController;