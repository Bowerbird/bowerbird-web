
window.Bowerbird.Models.Organisation = Backbone.Model.extend({
    url: '/members/organisation/',

    defaults: {
        name: '',
        description: '',
        website: '',
        avatar: ''
    },

    initialize: function (options) {
        _.extend(this, Backbone.Events);
        //this.avatar = new Bowerbird.Models.MediaResource();
        this.avatar = { id: '', url: '', altTag: '' };
    },

    toJSONViewModel: function () {
        return Backbone.Model.prototype.toJSON.call(this);
    }
});