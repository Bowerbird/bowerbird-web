
window.Bowerbird.Collections.Teams = Backbone.Collection.extend({
    model: Bowerbird.Models.Team,

    url: '/teams/list',

    initialize: function () {
        _.extend(this, Backbone.Events);
    }
});