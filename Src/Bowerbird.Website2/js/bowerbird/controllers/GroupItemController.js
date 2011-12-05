// inherits from StreamItemController

function GroupItemController() {
    StreamItemController.call();
}

GroupItemController.prototype = new StreamItemController();
GroupItemController.prototype.constructor = GroupItemController;