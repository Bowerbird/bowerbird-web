// inherits from StreamItemController

function CommentItemController() {
    StreamItemController.call();
}

CommentItemController.prototype = new StreamItemController();
CommentItemController.prototype.constructor = CommentItemController;