
window.Bowerbird.Models.Team = Backbone.Model.extend({
    toJSONViewModel: function () {
        return Backbone.Model.prototype.toJSON.call(this);
    }
}); 