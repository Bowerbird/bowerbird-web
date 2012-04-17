
window.Bowerbird.Views.AvatarItemView = Backbone.View.extend({
    className: 'avatar-uploaded',

    events: {
        'click .view-media-resource-button': 'viewMediaResource',
        'click .add-caption-button': 'viewMediaResource',
        'click .remove-media-resource-button': 'removeMediaResource'
    },

    initialize: function (options) {
        _.extend(this, Backbone.Events);
        _.bindAll(this, 'showTempMedia', 'showUploadedMedia');
        this.mediaResource = options.MediaResource;
        this.mediaResource.on('change:mediumImageUri', this.showUploadedMedia);
    },

    render: function () {
        this.$el.append(ich.avataruploaded(this.MediaResource.toJSON()));
        return this;
    },

    viewMediaResource: function () {
        alert('Coming soon');
    },

    removeMediaResource: function () {
        this.remove();
        $('#avatar-add-pane').show();
    },

    showTempMedia: function (img) {
        this.$el.find('div:first-child img').replaceWith($(img));
    },

    showUploadedMedia: function (mediaResource) {
        this.$el.find('div:first-child img').replaceWith($('<img src="' + mediaResource.get('MediumImageUri') + '" alt="" />'));
    }
});