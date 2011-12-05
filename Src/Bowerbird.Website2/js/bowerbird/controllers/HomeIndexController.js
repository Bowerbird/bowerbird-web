Bowerbird.HomeIndexController = function (filters, selectedFilter) {
    Bowerbird.PageController.call(this);

    this.workspaceControllers = [];

    this.headerController = new Bowerbird.HeaderController();
    this.workspaceNavController = new Bowerbird.WorkspaceNavController(filters);

    this.makeWorkspace(Bowerbird.StreamController, selectedFilter, this);

    var self = this;

    this.workspaceNavController.addEventCallback(
        'filterSelected',
        this,
        function (event) {
            self.onFilterSelected(event, self);
        }
    );
}

Bowerbird.HomeIndexController.prototype = new Bowerbird.PageController();
Bowerbird.HomeIndexController.prototype.constructor = Bowerbird.HomeIndexController;

Bowerbird.HomeIndexController.prototype.makeWorkspace = function (workspaceControllerType, filter, self) {
    var workspaceController = new workspaceControllerType(filter);

    self.workspaceControllers.push(workspaceController);

    // Build workspace from template
    if (workspaceController instanceof Bowerbird.StreamController) {
        $("#stream-template").tmpl(workspaceController).appendTo("#workspace-column");
    } else {
        $("#" + workspaceController.workspaceId + '-template').tmpl(workspaceController).appendTo("#workspace-column");
    }

    var $workspace = $('#' + workspaceController.workspaceId);

    // Set the workspace outside of current view
    $workspace.css({ left: -1600 });

    if (workspaceController instanceof Bowerbird.StreamController) {
        workspaceController.addEventCallback(
            'showCreateObservationForm',
            self,
            function (event) {
                self.onShowCreateObservationForm(event, self);
            }
        );
    }

    if (workspaceController instanceof Bowerbird.FormController) {
        workspaceController.addEventCallback(
            'hideForm',
            self,
            function (event) {
                self.onHideForm(event, self);
            }
        );
     }

    // Bring workspace to top of stack
    $workspace.css({ zIndex: self.workspaceControllers.length });

    // Slide workspace into view
    $workspace.animate({
        left: parseInt($workspace.css('left'), 10) == 0 ? -$workspace.outerWidth() : 0
    });

    return workspaceController;
}

Bowerbird.HomeIndexController.prototype.destroyWorkspace = function (workspaceId, self) {
    var $workspace = $('#' + workspaceId);

    // Slide workspace out of view
    $workspace.animate({
        left: parseInt($workspace.css('left'), 10) == 0 ? -$workspace.outerWidth() : 0
    },
    function () {
        $workspace.remove();
    });

    var workspaceController = self.workspaceControllers.pop();

    workspaceController.unbind();
}

Bowerbird.HomeIndexController.prototype.onFilterSelected = function (event, self) {
    this.makeWorkspace(Bowerbird.StreamController, event.selectedFilter, self);
//    var previousWorkspaceController = self.workspaceControllers.pop();

//    previousWorkspaceController.removeAllEventCallbacks();

//    var streamContoller = new Bowerbird.StreamController(event.selectedFilter);

//    streamContoller.addEventCallback(
//        'showCreateObservationForm',
//        self,
//        function (event) {
//            self.onShowCreateObservationForm(event, self);
//        }
//    );

//    self.workspaceControllers.push(streamContoller);

//    // Todo: do the slide in/out animation here
//    var $workspaceItem = $('#' + event.selectedFilter.key + '-stream');

//    //if ($workspaceItem.css('zIndex') == -1) {
//        $workspaceItem.css({ zIndex: 1 });
//    //} else {
//    //    $workspaceItem.css({ zIndex: -1 });
//    //}

//    $workspaceItem.animate({
//        left: parseInt($workspaceItem.css('left'), 10) == 0 ? -$workspaceItem.outerWidth() : 0
//    },
//    function () {
////        if ($(this).css('zIndex') == -1) {
////            $(this).css({ zIndex: 1 });
////        } else {
//            $(this).css({ zIndex: -1 });
////        }
//    });
}

Bowerbird.HomeIndexController.prototype.onShowCreateObservationForm = function (event, self) {
    this.makeWorkspace(Bowerbird.ObservationCreateFormController, event.selectedFilter, self);
}

Bowerbird.HomeIndexController.prototype.onHideForm = function (event, self) {
    this.destroyWorkspace(event.workspaceId, self);
}