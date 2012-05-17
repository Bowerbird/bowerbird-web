/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ProjectFormLayoutView
// -------------------------

define(['jquery', 'underscore', 'backbone', 'app', 'ich', 'views/editavatarview', 'multiselect'], function ($, _, Backbone, app, ich, EditAvatarView) {

    var ProjectFormLayoutView = Backbone.Marionette.Layout.extend({

        tagName: 'section',

        className: 'form single-medium',

        id: 'project-form',

        template: 'ProjectForm',

        regions: {
            avatar: '#avatar-fieldset'
        },

        events: {
            'click #cancel': '_cancel',
            'click #save': '_save',
            'change input#name': '_contentChanged',
            'change textarea#description': '_contentChanged',
            'change input#website': '_contentChanged',
            'click #avatar-import-button': '_showImportAvatar',
            'change #team-field input:checkbox': '_teamChanged'
        },

        onRender: function () {
            var editAvatarView = new EditAvatarView({ el: '#avatar-fieldset' });
            this.avatar.show(editAvatarView);
            editAvatarView.render();
        },

        _showImportAvatar: function () {
            alert('Coming soon');
        },

        _contentChanged: function (e) {
            var target = $(e.currentTarget);
            var data = {};
            data[target.attr('id')] = target.attr('value');
            this.model.set(data);
        },

        _teamChanged: function (e) {
            var $checkbox = $(e.currentTarget);
            if ($checkbox.attr('checked') === 'checked') {
                this.model.set('Team', $checkbox.attr('value'));
            } else {
                this.model.set('Team', '');
            }
        },

        _cancel: function () {
        },

        _save: function () {
            this.model.save();
        }
    });

    return ProjectFormLayoutView;
});