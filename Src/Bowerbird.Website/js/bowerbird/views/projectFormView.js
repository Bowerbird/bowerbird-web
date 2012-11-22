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
            'change input#Website': '_contentChanged'
        },

        initialize: function (options) {
            if (this.model.id) {
                this.viewEditMode = 'update';
            } else {
                this.viewEditMode = 'create';
            }            
        },

        serializeData: function () {
            return {
                Model: {
                    Project: this.model.toJSON()
                }
            };
        },

        onShow: function () {
            this._showDetails();
        },

        showBootstrappedDetails: function () {
            this.initializeRegions();
            this._showDetails();
        },

        _showDetails: function () {
            var editAvatarView = new EditAvatarView({ el: '.avatar-field', model: this.model });
            editAvatarView.render();
        },

        _contentChanged: function (e) {
            var target = $(e.currentTarget);
            var data = {};
            data[target.attr('id')] = target.attr('value');
            this.model.set(data);
        },

        _cancel: function () {
            app.showPreviousContentView();
        },

        _save: function () {
            if (this.viewEditMode == 'update') {
                this.model.set('Id', this.model.id.replace('projects/', ''));
            }            

            this.model.save();
            app.showPreviousContentView();
        }
    });

    return ProjectFormView;
});