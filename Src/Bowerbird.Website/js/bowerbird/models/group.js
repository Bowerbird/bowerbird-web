
window.Bowerbird.Models.Group = Backbone.Model.extend({
    
    defaults: {
        name: '',
        description: '',
        website: '',
        type: '',
        avatar: { id: '', url: '', altTag: '' }
    },

    initialize: function () {
        _.extend(this, Backbone.Events);
        _.bindAll(this);
    }

});