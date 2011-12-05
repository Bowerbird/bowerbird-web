// inherits from StreamItemController

function IdentificationItemController() {
    StreamItemController.call();
}

IdentificationItemController.prototype = new StreamItemController();
IdentificationItemController.prototype.constructor = IdentificationItemController;
IdentificationItemController.prototype.baseClass = StreamItemController.prototype.constructor;