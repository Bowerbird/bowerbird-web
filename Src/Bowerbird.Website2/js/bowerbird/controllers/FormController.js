Bowerbird.FormController = function (workspaceId) {
    Bowerbird.WorkspaceController.call(this, workspaceId);

    var self = this;

    $('#' + this.workspaceId + ' #cancel-form-button').live('click', function () {
        self.hideForm(self.workspaceId);
    });
}

Bowerbird.FormController.prototype = new Bowerbird.WorkspaceController();
Bowerbird.FormController.prototype.constructor = Bowerbird.FormController;

Bowerbird.FormController.prototype.hideForm = function (workspaceId) {
    var event = {
        name: 'hideForm',
        workspaceId: workspaceId
    };

    this.notifyEventCallbacks(event);
}

Bowerbird.FormController.prototype.unbind = function () {
    $('#' + this.workspaceId + ' #cancel-form-button').unbind('click');

    this.removeAllEventCallbacks();
}