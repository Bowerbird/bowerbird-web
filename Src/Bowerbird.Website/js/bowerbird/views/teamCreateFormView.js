
window.Bowerbird.Views.TeamCreateFormView = Backbone.View.extend({
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
        this.team = options.team;
        this.editAvatarView = new Bowerbird.Views.EditAvatarView({ el: $('#media-resources-fieldset'), observation: this.observation });
    },

    render: function () {
        var teamTemplate = ich.teamcreate({ team: app.get('newTeam').toJSON() });
        this.$el.append(teamTemplate);
        window.scrollTo(0, 0);
        return this;
    },

    start: function () {
        //var myScroll = new iScroll('media-uploader', { hScroll: true, vScroll: false });
    },

    _cancel: function () {
        app.set('newTeam', null);
        this.remove();
        app.appRouter.navigate(app.stream.get('uri'), { trigger: true });
    },

    _contentChanged: function (e) {
        var target = $(e.currentTarget);
        var data = {};
        data[target.attr('name')] = target.attr('value');
        this.team.set(data);
    },

    _save: function () {
        //alert('Coming soon');
        this.team.save();
        //this.remove();
        //app.appRouter.navigate(app.stream.get('uri'), { trigger: true });
    }
});