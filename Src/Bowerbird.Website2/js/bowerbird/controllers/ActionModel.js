// inherits from Model

function ActionModel(name) {
    Model.call();
    this.Name = name;
}

ActionModel.prototype = new Model();
ActionModel.prototype.constructor = ActionModel;