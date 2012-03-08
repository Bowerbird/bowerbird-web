
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
        this.mediaResource = options.mediaResource;
    },

    render: function () {
        $.tmpl('observationCreateMediaResourceUploadedTemplate', this.mediaResource.toJSON()).appendTo(this.$el);
        window.scrollTo(0, 0);
        return this;
    },

    viewMediaResource: function () {
        
    },

    removeMediaResource: function () {
    }
});