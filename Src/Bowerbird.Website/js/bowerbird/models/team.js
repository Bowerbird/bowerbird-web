
window.Bowerbird.Models.Team = Backbone.Model.extend({
    
    url: '/team/',

    initialize: function (options) {
        _.extend(this, Backbone.Events);
        this.constructor.__super__.initialize.apply(this, options);
        this.type = 'team';
    },

    toJSON: function () {
        return {
            name: this.get('name'),
            description: this.get('description'),
            website: this.get('website'),
            avatar: this.get('avatar').id,
            type: this.get('type'),
            organisation: this.get('organisation')
        };
    },

    toJSONViewModel: function () {
        return Backbone.Model.prototype.toJSON.call(this);
    }
}); 