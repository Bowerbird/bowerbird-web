
window.Bowerbird.Collections.Teams = Backbone.Collection.extend({
    
    model: Bowerbird.Models.Team,

    url: '/teams/list',

    initialize: function () {
        _.extend(this, Backbone.Events);
    },

    toJSONViewModel: function () {
        var viewModels = [];
        _.each(this.models, function (team) {
            viewModels.push(team.toJSONViewModel());
        });
        return viewModels;
    }
});