Bowerbird.PartialController = function () {
    Bowerbird.Controller.call(this);
}

Bowerbird.PartialController.prototype = new Bowerbird.Controller();
Bowerbird.PartialController.prototype.constructor = Bowerbird.PartialController;

