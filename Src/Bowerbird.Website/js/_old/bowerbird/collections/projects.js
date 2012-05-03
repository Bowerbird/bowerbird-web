
window.Bowerbird.Collections.Projects = Backbone.Collection.extend({
    model: Bowerbird.Models.Project,

    url: '/projects/list',

    initialize: function () {
        _.extend(this, Backbone.Events);
    },

    toJSONViewModel: function () {
        var viewModels = [];
        _.each(this.models, function (project) {
            viewModels.push(project.toJSONViewModel());
        });
        return viewModels;
    }

});