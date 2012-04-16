
window.Bowerbird.Views.MediaResourceItemView = Backbone.View.extend({
    className: 'media-resource-uploaded',

    events: {
        'click .view-media-resource-button': 'viewMediaResource',
        'click .add-caption-button': 'viewMediaResource',
        'click .remove-media-resource-button': 'removeMediaResource'
    },

    initialize: function (options) {
        _.extend(this, Backbone.Events);
        _.bindAll(this, 'showTempMedia', 'showUploadedMedia');
        this.MediaResource = options.MediaResource;
        this.MediaResource.on('change:mediumImageUri', this.showUploadedMedia);
    },

    render: function () {
        var mediaResourceUploaded = ich.observationmediaresourceuploaded(this.MediaResource.toJSON()).appendTo(this.$el);
        window.scrollTo(0, 0);
        return this;
    },

    viewMediaResource: function () {
        alert('Coming soon');
    },

    removeMediaResource: function () {
        var addToRemoveList = false;
        if (app.get('newObservation').MediaResources.find(function (mr) { return mr.id == this.MediaResource.Id; }) != null) {
            addToRemoveList = true;
        }
        app.get('newObservation').addMediaResources.remove(this.MediaResource.Id);
        app.get('newObservation').MediaResources.remove(this.MediaResource.Id);
        if (addToRemoveList) {
            app.get('newObservation').RemoveMediaResources.add(this.MediaResource);
        }

        this.remove();
    },

    showTempMedia: function (img) {
        this.$el.find('div:first-child img').replaceWith($(img));
    },

    showUploadedMedia: function (mediaResource) {
        this.$el.find('div:first-child img').replaceWith($('<img src="' + mediaResource.get('MediumImageUri') + '" alt="" />'));
    }
});