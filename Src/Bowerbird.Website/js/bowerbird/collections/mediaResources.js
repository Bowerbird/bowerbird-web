
window.Bowerbird.Collections.MediaResources = Backbone.Collection.extend({
    
    model: Bowerbird.Models.MediaResource,

    url: '/mediaresources/list',

    initialize: function () {
        _.extend(this, Backbone.Events);
    }
});