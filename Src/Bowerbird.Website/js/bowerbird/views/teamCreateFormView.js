
window.Bowerbird.Views.TeamCreateFormView = Backbone.View.extend({
    tagName: 'section',

    className: 'form single-medium',

    events: {
        'click #cancel': '_cancel',
        'click #save': '_save',
        'change input#name': '_contentChanged',
        'change textarea#description': '_contentChanged',
        'change input#website': '_contentChanged',
        'change #organisation-field input:checkbox': '_organisationChanged'
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
        this.appView = options.AppView;
        this.Team = options.Team;
        this.Organisation = options.Organisation;
        this.editAvatarView = new Bowerbird.Views.EditAvatarView({ el: $('#media-resources-fieldset'), Group: this.Team });
    },

    render: function () {
        var teamTemplate = ich.teamcreate({ Team: app.get('newTeam').toJSON() }).appendTo(this.$el);
        return this;
    },

    start: function () {
        this.editAvatarView.render();
        var organisations = new Bowerbird.Collections.Organisations();
        $.getJSON('organisation/list?HasAddTeamPermission=true', function (data) {
            organisations.reset(data);
            if (organisations.length > 0) {
                $.tmpl('<option value="${Id}">${Name}</option>', organisations.toJSONViewModel()).appendTo('#organisations');
                this.organisationListSelectView = $("#organisations").multiSelect({
                    selectAll: false,
                    singleSelect: true,
                    noneSelected: 'Select Organisation',
                    renderOption: function (id, option) {
                        var html = '<label><input style="display:none;" type="checkbox" name="' + id + '[]" value="' + option.value + '"';
//                        if (option.selected) {
//                            html += ' checked="checked"';
//                        }
                        var organisation = organisations.get(option.value);
                        html += ' /><img src="' + organisation.get('Avatar').UrlToImage + '" />' + organisation.get('Name') + '</label>';
                        return html;
                    },
                    oneOrMoreSelected: function (selectedOptions) {
                        var $selectedHtml = $('<span />');
                        _.each(selectedOptions, function (option) {
                            var organisation = organisations.get(option.value);
                            $selectedHtml.append('<span><img src="' + organisation.get('Avatar').UrlToImage + '" />' + organisation.get('Name') + '</span> ');
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

    _organisationChanged: function (e) {
        var $checkbox = $(e.currentTarget);
        if ($checkbox.attr('checked') === 'checked') {
            this.team.set('Organisation', $checkbox.attr('value'));
        } else {
            this.team.set('Organisation', '');
        }
    },

    _cancel: function () {
        app.set('newTeam', null);
        app.appRouter.navigate(app.stream.get('Uri'), { trigger: false });
        this.trigger('formClosed', this);
    },

    _contentChanged: function (e) {
        var target = $(e.currentTarget);
        var data = {};
        data[target.attr('name')] = target.attr('value');
        this.Team.set(data);
    },

    _save: function () {
        this.Team.save();
        app.appRouter.navigate(app.stream.get('Uri'), { trigger: false });
        this.trigger('formClosed', this);
    }
});