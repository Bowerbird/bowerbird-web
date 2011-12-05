Bowerbird.StreamController = function (filter) {
    Bowerbird.WorkspaceController.call(this, filter.key + '-stream');

    this.streamItemControllers = [];
    this.filter = filter;

    var self = this;

    $('#' + this.workspaceId + ' #create-observation-button').live('click', function () {
        self.showCreateObservationForm();
    });
}

Bowerbird.StreamController.prototype = new Bowerbird.WorkspaceController();
Bowerbird.StreamController.prototype.constructor = Bowerbird.StreamController;

Bowerbird.StreamController.prototype.showCreateObservationForm = function () {
    var event = {
        name: 'showCreateObservationForm',
        selectedFilter: this.filter
    };

    this.notifyEventCallbacks(event);
}

Bowerbird.StreamController.prototype.unbind = function () {
    $('#' + this.workspaceId + ' #create-observation-button').unbind('click');

    this.removeAllEventCallbacks();
}