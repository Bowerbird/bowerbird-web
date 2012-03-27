
window.Bowerbird.Models.Project = Backbone.Model.extend({
    url: '/members/project/',

    defaults: {
        name: '',
        description: '',
        website: '',
        avatar: null
    },

    initialize: function (options) {
        _.extend(this, Backbone.Events);
        this.avatar = new Bowerbird.Models.MediaResource();
    },

    toJSON: function () {
        return {
            name: this.get('name'),
            description: this.get('description'),
            website: this.get('website')
        };
    }
});