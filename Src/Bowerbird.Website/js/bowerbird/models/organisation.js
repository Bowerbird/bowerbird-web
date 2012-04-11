
window.Bowerbird.Models.Organisation = Bowerbird.Models.Group.extend({

    url: '/organisation/',

    initialize: function (options) {
        _.extend(this, Backbone.Events);
        this.constructor.__super__.initialize.apply(this, options);
        this.type = 'organisation';
    },

    toJSON: function () {
        return {
            name: this.get('name'),
            description: this.get('description'),
            website: this.get('website'),
            avatar: this.get('avatar').id,
            type: this.get('type')
        };
    },

    toJSONViewModel: function () {
        return Backbone.Model.prototype.toJSON.call(this);
    }
});