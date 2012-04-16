
window.Bowerbird.Models.Organisation = Bowerbird.Models.Group.extend({

    url: '/organisation/',

    initialize: function (options) {
        _.extend(this, Backbone.Events);
        this.constructor.__super__.initialize.apply(this, options);
        this.type = 'organisation';
    },

    toJSON: function () {
        return {
            Name: this.get('Name'),
            Description: this.get('Description'),
            Website: this.get('Website'),
            Avatar: this.get('Avatar').Id,
            Type: this.get('Type')
        };
    },

    toJSONViewModel: function () {
        return Backbone.Model.prototype.toJSON.call(this);
    }
});