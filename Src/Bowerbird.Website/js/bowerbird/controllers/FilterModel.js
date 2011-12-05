// inherits from Model

function FilterModel(name) {
    Model.call();
    this.Name = name;
}

FilterModel.prototype = new Model();
FilterModel.prototype.constructor = FilterModel;