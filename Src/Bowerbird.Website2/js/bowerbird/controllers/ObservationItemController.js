// inherits from StreamItemController

function ObservationItemController() {
    StreamItemController.call();
}

ObservationItemController.prototype = new StreamItemController();
ObservationItemController.prototype.constructor = ObservationItemController;
ObservationItemController.prototype.baseClass = StreamItemController.prototype.constructor;