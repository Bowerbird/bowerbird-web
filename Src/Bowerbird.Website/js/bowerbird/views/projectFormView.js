/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/jquery/jquery.fileupload.js" />
/// <reference path="../../libs/jquery/load-image.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ProjectFormView
// ---------------

define(['jquery', 'underscore', 'backbone', 'app', 'ich', 'loadimage', 'views/editavatarview', 'fileupload', 'multiselect'], 
function ($, _, Backbone, app, ich, loadImage, EditAvatarView) {

    var ProjectFormView = Backbone.Marionette.Layout.extend({
        viewType: 'form',

        className: 'form form-medium single project-form',

        template: 'ProjectForm',

        regions: {
            avatar: '#avatar-fieldset'
        },

        events: {
            'click #cancel': '_cancel',
            'click #save': '_save',
            'change input#Name': '_contentChanged',
            'change textarea#Description': '_contentChanged',
            'change input#Website': '_contentChanged',
            'change #team-field input:checkbox': '_teamChanged'
        },

        initialize: function (options) {
            this.teams = options.teams;
        },

        serializeData: function () {
            return {
                Model: {
                    Project: this.model.toJSON(),
                    Teams: this.teams
                }
            };
        },

        onShow: function () {
            this._showDetails();
        },

        showBootstrappedDetails: function () {
            this.initializeRegions();
            //this.$el = $('#content .project-form');
            this._showDetails();
        },

        _showDetails: function () {
            var editAvatarView = new EditAvatarView({ el: '#avatar-fieldset', model: this.model });
            editAvatarView.render();

            this.teamListSelectView = this.$el.find("#TeamId").multiSelect({
                selectAll: false,
                singleSelect: true,
                noOptionsText: 'No Teams',
                noneSelected: 'Select a Team',
                oneOrMoreSelected: function (selectedOptions) {
                    var $selectedHtml = $('<span />');
                    _.each(selectedOptions, function (option) {
                        $selectedHtml.append('<span>' + option.text + '</span> ');
                    });
                    return $selectedHtml.children();
                }
            });
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
                this.model.set('TeamId', $checkbox.attr('value'));
            } else {
                this.model.set('TeamId', '');
            }
        },

        _cancel: function () {
            app.showPreviousContentView();
        },

        _save: function () {
            this.model.save();
            app.showPreviousContentView();
        }
    });

    return ProjectFormView;
});