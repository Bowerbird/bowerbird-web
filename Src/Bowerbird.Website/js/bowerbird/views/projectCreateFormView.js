
window.Bowerbird.Views.ProjectCreateFormView = Backbone.View.extend({
    tagName: 'section',

    className: 'form single-medium',

    events: {
        'click #cancel': '_cancel',
        'click #save': '_save',
        'change input#name': '_contentChanged',
        'change input#description': '_contentChanged',
        'change input#website': '_contentChanged'
    },

    initialize: function (options) {
        _.extend(this, Backbone.Events);
        this.appView = options.appView;
        this.project = options.project;
        this.editAvatarView = new Bowerbird.Views.EditAvatarView({ el: $('#media-resources-fieldset'), parent: this.project });
    },

    render: function () {
        var projectTemplate = ich.projectcreate({ project: app.get('newProject').toJSON() });
        //var projectTemplate = ich.projectcreate({});
        this.$el.append(projectTemplate);
        window.scrollTo(0, 0);
        return this;
    },

    start: function () {
        //var myScroll = new iScroll('media-uploader', { hScroll: true, vScroll: false });
    },

    _cancel: function () {
        app.set('newProject', null);
        this.remove();
        app.appRouter.navigate(app.stream.get('uri'), { trigger: true });
    },

    _contentChanged: function (e) {
        var target = $(e.currentTarget);
        var data = {};
        data[target.attr('name')] = target.attr('value');
        this.project.set(data);
    },

    _save: function () {
        //alert('Coming soon');
        this.project.save();
        //this.remove();
        //app.appRouter.navigate(app.stream.get('uri'), { trigger: true });
    }
});