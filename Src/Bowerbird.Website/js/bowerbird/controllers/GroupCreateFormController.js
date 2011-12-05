// inherits from FormController

function GroupCreateFormController() {
    FormController.call();
}

GroupCreateFormController.prototype = new FormController();
GroupCreateFormController.prototype.constructor = GroupCreateFormController;