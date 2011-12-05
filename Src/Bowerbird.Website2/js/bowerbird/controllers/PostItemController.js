// inherits from StreamItemController

function PostItemController() {
    StreamItemController.call();
}

PostItemController.prototype = new StreamItemController();
PostItemController.prototype.constructor = PostItemController;
PostItemController.prototype.baseClass = StreamItemController.prototype.constructor;