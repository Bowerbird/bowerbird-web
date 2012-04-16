
window.Bowerbird.Collections.Organisations = Backbone.Collection.extend({
    
    model: Bowerbird.Models.Organisation,

    url: '/organisations/list',

    initialize: function () {
        _.extend(this, Backbone.Events);
    },

    toJSONViewModel: function () {
        var viewModels = [];
        _.each(this.models, function (organisation) {
            viewModels.push(organisation.toJSONViewModel());
        });
        return viewModels;
    }
});