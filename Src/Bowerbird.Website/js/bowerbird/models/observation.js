
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
        this.Projects = new Bowerbird.Collections.Projects();
        this.AddMediaResources = new Bowerbird.Collections.MediaResources();
        this.MediaResources = new Bowerbird.Collections.MediaResources();
        this.RemoveMediaResources = new Bowerbird.Collections.MediaResources();
        if (_.has(options, 'MediaResources')) {
            this.MediaResources.reset(options.MediaResources);
        }
    },

    allMediaResources: function () {
        return new Bowerbird.Collections.MediaResources(this.AddMediaResources.models).add(this.MediaResources.models).toArray();
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
            Projects: this.Projects.pluck('Id'),
            AddMedia: this.AddMediaResources.map(function (mediaResource) {
                    return { MediaResourceId: mediaResource.Id, Description: 'stuff', Licence: 'licenceX' };
                }),
            Media: this.MediaResources.map(function (mediaResource) {
                    return { MediaResourceId: mediaResource.id, Description: 'stuff', Licence: 'licenceX' };
                }),
            RemoveMedia: this.RemoveMediaResources.pluck('id')
        };
    }
});