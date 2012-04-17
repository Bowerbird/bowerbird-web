
window.Bowerbird.Models.Project = Backbone.Model.extend({
    
    url: '/project/',

    initialize: function (options) {
        _.extend(this, Backbone.Events);
        this.constructor.__super__.initialize.apply(this, [options]);
        this.Type = 'project';
    },

    toJSON: function () {
        return {
            Name: this.get('Name'),
            Description: this.get('Description'),
            Website: this.get('Website'),
            Avatar: this.get('Avatar').id,
            Team: this.get('Team'),
            Type: this.get('Type')
        };
    },

    toJSONViewModel: function () {
        return Backbone.Model.prototype.toJSON.call(this);
    }
});