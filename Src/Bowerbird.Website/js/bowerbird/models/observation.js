
window.Bowerbird.Models.Observation = Backbone.Model.extend({
    defaults: {
        title: '',
        observedOn: null,
        address: '',
        latitude: null,
        longitude: null,
        category: '',
        anonymiseLocation: false
    },

    initialize: function (options) {
        _.extend(this, Backbone.Events);
        this.projects = new Bowerbird.Collections.Projects();
        this.addMediaResources = new Bowerbird.Collections.MediaResources();
        this.mediaResources = new Bowerbird.Collections.MediaResources();
        this.removeMediaResources = new Bowerbird.Collections.MediaResources();
        if (_.has(options, 'mediaResources')) {
            this.mediaResources.reset(options.mediaResources);
        }
    },

    allMediaResources: function () {
        return new Bowerbird.Collections.MediaResources(this.addMediaResources.models).add(this.mediaResources.models).toArray();
    }
});