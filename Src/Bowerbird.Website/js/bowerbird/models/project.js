
window.Bowerbird.Models.Project = Backbone.Model.extend({
    
    url: '/members/project/',

    initialize: function (options) {
        _.extend(this, Backbone.Events);
        this.constructor.__super__.initialize.apply(this, options);
        this.type = 'project';
    },

    toJSON: function () {
        return {
            name: this.get('name'),
            description: this.get('description'),
            website: this.get('website'),
            avatar: this.get('avatar').id,
            team: this.get('team'),
            type: this.get('type')
        };
    },

    toJSONViewModel: function () {
        return Backbone.Model.prototype.toJSON.call(this);
    }
});