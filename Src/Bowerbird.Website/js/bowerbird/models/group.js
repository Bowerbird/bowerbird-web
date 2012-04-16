
window.Bowerbird.Models.Group = Backbone.Model.extend({
    
    defaults: {
        Name: '',
        Description: '',
        Website: '',
        Type: '',
        Avatar: { Id: '', Url: '', AltTag: '' }
    },

    initialize: function () {
        _.extend(this, Backbone.Events);
        _.bindAll(this);
    }

});