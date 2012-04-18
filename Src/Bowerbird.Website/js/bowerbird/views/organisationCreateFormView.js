
window.Bowerbird.Views.OrganisationCreateFormView = Backbone.View.extend({
    tagName: 'section',

    className: 'form single-medium',

    events: {
        'click #cancel': '_cancel',
        'click #save': '_save',
        'change input#name': '_contentChanged',
        'change textarea#description': '_contentChanged',
        'change input#website': '_contentChanged'
    },

    initialize: function (options) {
        _.extend(this, Backbone.Events);
        _.bindAll(this,
        'render',
        'start',
        '_cancel',
        '_contentChanged',
        '_save'
        );
        this.organisation = options.organisation;
        this.editAvatarView = new Bowerbird.Views.EditAvatarView({ el: $('#media-resources-fieldset'), group: this.organisation });
    },

    render: function () {
        this.$el.append(ich.OrganisationCreate({ Organisation: app.get('newOrganisation').toJSON() }));
        window.scrollTo(0, 0);
        return this;
    },

    start: function () {
        this.editAvatarView.render();
        //var myScroll = new iScroll('media-uploader', { hScroll: true, vScroll: false });
    },

    _cancel: function () {
        app.set('newOrganisation', null);
        app.appRouter.navigate(app.stream.get('Uri'), { trigger: false });
        this.trigger('formClosed', this);
    },

    _contentChanged: function (e) {
        var target = $(e.currentTarget);
        var data = {};
        data[target.attr('name')] = target.attr('value');
        this.organisation.set(data);
    },

    _save: function () {
        this.organisation.save();
        app.appRouter.navigate(app.stream.get('Uri'), { trigger: false });
        this.trigger('formClosed', this);
    }
});