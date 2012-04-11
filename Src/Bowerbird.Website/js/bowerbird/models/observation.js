
window.Bowerbird.Models.Observation = Backbone.Model.extend({

    url: '/observation/',

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
    },

    toJSON: function () {
        return {
            title: this.get('title'),
            observedOn: this.get('observedOn'),
            address: this.get('address'),
            latitude: this.get('latitude'),
            longitude: this.get('longitude'),
            category: this.get('category'),
            anonymiseLocation: this.get('anonymiseLocation'),
            projects: this.projects.pluck('id'),
            addMedia: this.addMediaResources.map(function (mediaResource) {
                    return { mediaResourceId: mediaResource.id, description: 'stuff', licence: 'licenceX' };
                }),
            media: this.mediaResources.map(function (mediaResource) {
                    return { mediaResourceId: mediaResource.id, description: 'stuff', licence: 'licenceX' };
                }),
            removeMedia: this.removeMediaResources.pluck('id')
        };
    }
});