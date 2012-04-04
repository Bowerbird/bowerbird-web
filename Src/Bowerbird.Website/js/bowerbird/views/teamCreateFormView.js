
window.Bowerbird.Views.TeamCreateFormView = Backbone.View.extend({
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
        this.team = options.team;
        this.editAvatarView = new Bowerbird.Views.EditAvatarView({ el: $('#media-resources-fieldset'), group: this.team });
    },

    render: function () {
        var teamTemplate = ich.teamcreate({ team: app.get('newTeam').toJSON() }).appendTo(this.$el);
        //window.scrollTo(0, 0);
        return this;
    },

    start: function () {
        this.editAvatarView.render();
        var organisationsAdministered = new Bowerbird.Collections.Teams();
        $.getJSON('organisation/list?HasAddTeamPermission=true', function (data) {
            organisationsAdministered.reset(data);
            if (organisationsAdministered.length > 0) {
                $.tmpl('<option value="${id}">${name}</option>', organisationsAdministered).appendTo('#organisations');
                this.organisationListSelectView = $("#organisations").multiSelect({
                    selectAll: false,
                    singleSelect: true,
                    noneSelected: 'Select Team',
                    renderOption: function (id, option) {
                        var html = '<label><input style="display:none;" type="checkbox" name="' + id + '[]" value="' + option.value + '"';
                        if (option.selected) {
                            html += ' checked="checked"';
                        }
                        var selectedTeam = organisationsAdministered.get(option.value);
                        html += ' /><img src="' + selectedTeam.get('avatar').get('urlToImage') + '" />' + selectedTeam.get('name') + '</label>';
                        return html;
                    },
                    oneOrMoreSelected: function (selectedOptions) {
                        var selectedTeam = teams.get(option.value);
                        var $selectedHtml = $('<span />');
                        _.each(selectedOptions, function (option) {
                            $selectedHtml.append('<span><img src="' + selectedTeam.get('avatar').get('urlToImage') + '" />' + selectedTeam.get('name') + '</span> ');
                        });
                        return $selectedHtml;
                    }
                });
            }
            else {
                $('#organisation-field').remove();
            }
        });
    },

    _cancel: function () {
        app.set('newTeam', null);
        app.appRouter.navigate(app.stream.get('uri'), { trigger: false });
        this.trigger('formClosed', this);
    },

    _contentChanged: function (e) {
        var target = $(e.currentTarget);
        var data = {};
        data[target.attr('name')] = target.attr('value');
        this.team.set(data);
    },

    _save: function () {
        this.team.save();
        app.appRouter.navigate(app.stream.get('uri'), { trigger: false });
        this.trigger('formClosed', this);
    }
});