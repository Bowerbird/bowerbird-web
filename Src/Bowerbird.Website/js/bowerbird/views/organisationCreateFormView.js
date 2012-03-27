
window.Bowerbird.Views.OrganisationCreateFormView = Backbone.View.extend({
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
        this.organisation = options.organisation;
        this.editAvatarView = new Bowerbird.Views.EditAvatarView({ el: $('#media-resources-fieldset'), observation: this.observation });
    },

    render: function () {
        var organisationTemplate = ich.organisationcreate({ organisation: app.get('newOrganisation').toJSON() });
        this.$el.append(organisationTemplate);
        window.scrollTo(0, 0);
        return this;
    },

    start: function () {
        //var myScroll = new iScroll('media-uploader', { hScroll: true, vScroll: false });
    },

    _cancel: function () {
        app.set('newOrganisation', null);
        this.remove();
        app.appRouter.navigate(app.stream.get('uri'), { trigger: true });
    },

    _contentChanged: function (e) {
        var target = $(e.currentTarget);
        var data = {};
        data[target.attr('name')] = target.attr('value');
        this.organisation.set(data);
    },

    _save: function () {
        //alert('Coming soon');
        this.organisation.save();
        //this.remove();
        //app.appRouter.navigate(app.stream.get('uri'), { trigger: true });
    }
});