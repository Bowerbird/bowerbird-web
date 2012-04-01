
window.Bowerbird.Models.Project = Backbone.Model.extend({
    url: '/members/project/',

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

    toJSON: function () {
        return {
            name: this.get('name'),
            description: this.get('description'),
            website: this.get('website'),
            avatar: this.get('avatar').id
        };
    },

    toJSONViewModel: function () {
        return Backbone.Model.prototype.toJSON.call(this);
    }
});