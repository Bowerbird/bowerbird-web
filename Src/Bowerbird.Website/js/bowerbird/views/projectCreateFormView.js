
window.Bowerbird.Views.ProjectCreateFormView = Backbone.View.extend({
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
        this.appView = options.appView;
        this.project = options.project;
        this.editAvatarView = new Bowerbird.Views.EditAvatarView({ el: $('#media-resources-fieldset'), group: this.project });
    },

    render: function () {
        var projectTemplate = ich.projectcreate({ project: app.get('newProject').toJSON() }).appendTo(this.$el);
        return this;
    },

    start: function () {
        this.editAvatarView.render();
    },

    _cancel: function () {
        app.set('newProject', null);
        app.appRouter.navigate(app.stream.get('uri'), { trigger: true });
        this.trigger('formClosed', this);
    },

    _contentChanged: function (e) {
        var target = $(e.currentTarget);
        var data = {};
        data[target.attr('name')] = target.attr('value');
        this.project.set(data);
    },

    _save: function () {
        this.project.save();
        app.appRouter.navigate(app.stream.get('uri'), { trigger: true });
        this.trigger('formClosed', this);
    }
});