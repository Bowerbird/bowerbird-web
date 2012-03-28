
window.Bowerbird.Views.AvatarMediaResourceView = Backbone.View.extend({
    className: 'avatar-media-resource-uploaded',

    events: {
        'click .view-media-resource-button': 'viewMediaResource',
        'click .add-caption-button': 'viewMediaResource',
        'click .remove-media-resource-button': 'removeMediaResource'
    },

    initialize: function (options) {
        _.extend(this, Backbone.Events);
        _.bindAll(this,
        'showTempMedia',
        'showUploadedMedia'
        );
        this.mediaResource = options.mediaResource;
        this.parent = options.parent;
        this.mediaResource.on('change:mediumImageUri', this.showUploadedMedia);
    },

    render: function () {
        var avatarUploaded = ich.avataruploaded({ avatar: this.mediaResource.toJSON() });
        this.$el.append(projectTemplate);
        window.scrollTo(0, 0);
        return this;
    },

    viewMediaResource: function () {
        alert('Coming soon');
    },

    removeMediaResource: function () {
        this.parent.avatar = null;
        this.remove();
    },

    showTempMedia: function (img) {
        this.$el.find('div:first-child img').replaceWith($(img));
    },

    showUploadedMedia: function (mediaResource) {
        this.$el.find('div:first-child img').replaceWith($('<img src="' + mediaResource.get('mediumImageUri') + '" alt="" />'));
    }
});