
window.Bowerbird.Models.Observation = Backbone.Model.extend({
    url: '/observation/',

    defaults: {
        Title: '',
        ObservedOn: null,
        Address: '',
        Latitude: null,
        Longitude: null,
        Category: '',
        AnonymiseLocation: false
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
            Title: this.get('Title'),
            ObservedOn: this.get('ObservedOn'),
            Address: this.get('Address'),
            Latitude: this.get('Latitude'),
            Longitude: this.get('Longitude'),
            Category: this.get('Category'),
            AnonymiseLocation: this.get('AnonymiseLocation'),
            Projects: this.projects.pluck('Id'),
            AddMedia: this.addMediaResources.map(function (mediaResource) {
                    return { MediaResourceId: mediaResource.Id, Description: 'stuff', Licence: 'licenceX' };
                }),
            Media: this.mediaResources.map(function (mediaResource) {
                    return { MediaResourceId: mediaResource.Id, Description: 'stuff', Licence: 'licenceX' };
                }),
            RemoveMedia: this.removeMediaResources.pluck('id')
        };
    }
});