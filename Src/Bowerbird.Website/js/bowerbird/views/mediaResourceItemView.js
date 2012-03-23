
window.Bowerbird.Views.MediaResourceItemView = Backbone.View.extend({
    className: 'media-resource-uploaded',

    events: {
        'click .view-media-resource-button': 'viewMediaResource',
        'click .add-caption-button': 'viewMediaResource',
        'click .remove-media-resource-button': 'removeMediaResource'
    },

    template: $.template('observationCreateMediaResourceUploadedTemplate', $('#observation-create-media-resource-uploaded-template')),

    initialize: function (options) {
        _.extend(this, Backbone.Events);
        _.bindAll(this, 'showTempMedia', 'showUploadedMedia');
        this.mediaResource = options.mediaResource;
        this.mediaResource.on('change:mediumImageUri', this.showUploadedMedia);
    },

    render: function () {
        $.tmpl('observationCreateMediaResourceUploadedTemplate', this.mediaResource.toJSON()).appendTo(this.$el);
        window.scrollTo(0, 0);
        return this;
    },

    viewMediaResource: function () {
        alert('Coming soon');
    },

    removeMediaResource: function () {
        var addToRemoveList = false;
        if (app.get('newObservation').mediaResources.find(function (mr) { return mr.id == this.mediaResource.id; }) != null) {
            addToRemoveList = true;
        }
        app.get('newObservation').addMediaResources.remove(this.mediaResource.id);
        app.get('newObservation').mediaResources.remove(this.mediaResource.id);
        if (addToRemoveList) {
            app.get('newObservation').removeMediaResources.add(this.mediaResource);
        }

        this.remove();
    },

    showTempMedia: function (img) {
        this.$el.find('div:first-child img').replaceWith($(img));
    },

    showUploadedMedia: function (mediaResource) {
        this.$el.find('div:first-child img').replaceWith($('<img src="' + mediaResource.get('mediumImageUri') + '" alt="" />'));
    }
});