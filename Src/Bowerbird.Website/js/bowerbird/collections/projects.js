
window.Bowerbird.Collections.Projects = Backbone.Collection.extend({
    model: Bowerbird.Models.Project,

    url: '/projects/list',

    initialize: function () {
        _.extend(this, Backbone.Events);
    }
});