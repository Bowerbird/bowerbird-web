Bowerbird.WorkspaceNavController = function (filters) {
    Bowerbird.PartialController.call(this);

    this.filters = filters;

    var self = this;
    $('.filter-group input').click(function () {
        self.filterSelected($(this).data('filterKey'));
    });
}

Bowerbird.WorkspaceNavController.prototype = new Bowerbird.PartialController();
Bowerbird.WorkspaceNavController.prototype.constructor = Bowerbird.WorkspaceNavController;

Bowerbird.WorkspaceNavController.prototype.filterSelected = function (filterKey) {
    var selectedFilter = null;

    for (var i in this.filters) {
        if (this.filters[i].key === filterKey) {
            selectedFilter = this.filters[i];
            break;
        }
    }

    var event = {
        name: 'filterSelected',
        selectedFilter: selectedFilter
    };

    this.notifyEventCallbacks(event);
}

Bowerbird.WorkspaceNavController.prototype.unbind = function () {
    $('.filter-group input').unbind('click');

    this.removeAllEventCallbacks();
}