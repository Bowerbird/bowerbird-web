
window.Bowerbird.Models.Project = Backbone.Model.extend({
    url: '/projects/',

    initialize: function (options) {
        _.extend(this, Backbone.Events);
        this.constructor.__super__.initialize.apply(this, [options]);
    },

    toJSON: function () {
        return {
            Name: this.get('Name'),
            Description: this.get('Description'),
            Website: this.get('Website'),
            Avatar: this.get('Avatar').id,
            Team: this.get('Team'),
            Type: 'project'
        };
    },

    toJSONViewModel: function () {
        return Backbone.Model.prototype.toJSON.call(this);
    }
});