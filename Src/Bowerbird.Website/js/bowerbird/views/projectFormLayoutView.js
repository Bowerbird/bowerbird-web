/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/jquery/jquery.fileupload.js" />
/// <reference path="../../libs/jquery/load-image.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ProjectFormLayoutView
// -------------------------

define(['jquery', 'underscore', 'backbone', 'app', 'ich', 'loadimage', 'views/editavatarview', 'fileupload', 'multiselect'], 
function ($, _, Backbone, app, ich, loadImage, EditAvatarView) {

    var ProjectFormLayoutView = Backbone.Marionette.Layout.extend({

        className: 'form single-medium project-form',

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
            log('projectFormLayoutView:initialize');
            this.teams = options.teams;
        },

        serializeData: function () {
            log('projectFormLayoutView:serializeData');
            return {
                Model: {
                    Project: this.model.toJSON(),
                    Teams: this.teams
                }
            };
        },

        onShow: function () {
            log('projectFormLayoutView:onShow');
            this._showDetails();
        },

        showBootstrappedDetails: function () {
            log('projectFormLayoutView:showBootstrappedDetails');
            this.initializeRegions();
            this.$el = $('#content .project-form');
            this._showDetails();
        },

        _showDetails: function () {
            log('projectFormLayoutView:_showDetails');
            var editAvatarView = new EditAvatarView({ el: '#avatar-fieldset', model: this.model });
            editAvatarView.render();

            this.teamListSelectView = this.$el.find("#Team").multiSelect({
                selectAll: false,
                singleSelect: true,
                noOptionsText: 'No Teams',
                noneSelected: 'Select A Team',
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
            log('projectFormLayoutView:_contentChanged');
            var target = $(e.currentTarget);
            var data = {};
            data[target.attr('id')] = target.attr('value');
            this.model.set(data);
        },

        _teamChanged: function (e) {
            log('projectFormLayoutView:_teamChanged');
            var $checkbox = $(e.currentTarget);
            if ($checkbox.attr('checked') === 'checked') {
                this.model.set('Team', $checkbox.attr('value'));
            } else {
                this.model.set('Team', '');
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

    return ProjectFormLayoutView;
});