function BoundModel(name, viewModel) {
    Model.call();
    this.Name = name;
    this.ViewModel = viewModel;
}

BoundModel.prototype = new Model();
BoundModel.prototype.constructor = BoundModel;

BoundModel.prototype.ViewModel = null;
BoundModel.prototype.Bind = function () { ko.applyBindings(this.viewModel); };