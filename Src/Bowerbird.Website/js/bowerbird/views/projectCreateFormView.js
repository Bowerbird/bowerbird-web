
window.Bowerbird.Views.ProjectCreateFormView = Backbone.View.extend({
    tagName: 'section',

    className: 'form single-medium',

    events: {
        'click #cancel': '_cancel',
        'click #save': '_save',
        'change input#name': '_contentChanged',
        'change textarea#description': '_contentChanged',
        'change input#website': '_contentChanged',
        'change #team-field input:checkbox': '_teamChanged'
    },

    initialize: function (options) {
        _.extend(this, Backbone.Events);
        _.bindAll(this,
        'render',
        'start',
        '_cancel',
        '_contentChanged',
        '_save',
        '_teamChanged'
        );
        this.appView = options.AppView;
        this.Project = options.Project;
        this.Team = options.Team;
        this.editAvatarView = new Bowerbird.Views.EditAvatarView({ el: $('#media-resources-fieldset'), Group: this.Project });
    },

    render: function () {
        var projectTemplate = ich.projectcreate({ Project: app.get('newProject').toJSON() }).appendTo(this.$el);
        return this;
    },

    start: function () {
        this.editAvatarView.render();
        var teams = new Bowerbird.Collections.Teams();
        $.getJSON('team/list?HasAddProjectPermission=true', function (data) {
            teams.reset(data);
            if (teams.length > 0) {
                $.tmpl('<option value="${Id}">${Name}</option>', teams.toJSONViewModel()).appendTo('#team');
                this.teamListSelectView = $("#team").multiSelect({
                    selectAll: false,
                    singleSelect: true,
                    noneSelected: 'Select Team',
                    renderOption: function (id, option) {
                        var html = '<label><input style="display:none;" type="checkbox" name="' + id + '[]" value="' + option.value + '"';
//                        if (option.selected) {
//                            html += ' checked="checked"';
//                        }
                        var team = teams.get(option.value);
                        html += ' /><img src="' + team.get('Avatar').UrlToImage + '" />' + team.get('Name') + '</label>';
                        return html;
                    },
                    oneOrMoreSelected: function (selectedOptions) {
                        var $selectedHtml = $('<span />');
                        _.each(selectedOptions, function (option) {
                            var team = teams.get(option.value);
                            $selectedHtml.append('<span><img src="' + team.get('Avatar').UrlToImage + '" />' + team.get('Name') + '</span> ');
                        });
                        return $selectedHtml;
                    }
                });
            }
            else {
                $('#team-field').remove();
            }
        });
    },

    _teamChanged: function (e) {
        var $checkbox = $(e.currentTarget);
        if ($checkbox.attr('checked') === 'checked') {
            this.Project.set('Team', $checkbox.attr('value'));
        } else {
            this.Project.set('Team', '');
        }
    },

    _cancel: function () {
        app.set('newProject', null);
        app.appRouter.navigate(app.stream.get('Uri'), { trigger: false });
        this.trigger('formClosed', this);
    },

    _contentChanged: function (e) {
        var target = $(e.currentTarget);
        var data = {};
        data[target.attr('name')] = target.attr('value');
        this.Project.set(data);
    },

    _save: function () {
        this.Project.save();
        app.appRouter.navigate(app.stream.get('Uri'), { trigger: false });
        this.trigger('formClosed', this);
    }
});