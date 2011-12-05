Bowerbird.WorkspaceController = function (workspaceId) {
    Bowerbird.PartialController.call(this);

    this.workspaceId = workspaceId;
}

Bowerbird.WorkspaceController.prototype = new Bowerbird.PartialController();
Bowerbird.WorkspaceController.prototype.constructor = Bowerbird.WorkspaceController;
