// inherits from StreamItemController

function ProjectItemController() {
    StreamItemController.call();
}

ProjectItemController.prototype = new StreamItemController();
ProjectItemController.prototype.constructor = ProjectItemController;
ProjectItemController.prototype.baseClass = StreamItemController.prototype.constructor;