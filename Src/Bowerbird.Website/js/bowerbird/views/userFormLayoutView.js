/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// OrganisationFormLayoutView
// --------------------------

define(['jquery', 'underscore', 'backbone', 'app', 'ich', 'loadimage', 'views/editavatarview', 'fileupload', 'multiselect'],
function ($, _, Backbone, app, ich, loadImage, EditAvatarView) 
{
    var UserFormLayoutView = Backbone.Marionette.Layout.extend({

        className: 'form single-medium user-form',

        template: 'UserForm',

        regions: {
            avatar: '#avatar-fieldset'
        },

        events: {
            'click #cancel': '_cancel',
            'click #save': '_save',
            'change input#FirstName': '_contentChanged',
            'change input#LastName': '_contentChanged',
            'change input#Email': '_contentChanged'
        },

        initialize: function (options) {
            log('userFormLayoutView.initialize');
        },

        serializeData: function () {
            log('userFormLayoutView:serializeData');
            return {
                Model: {
                    User: this.model.toJSON()
                }
            };
        },

        onShow: function () {
            log('userFormLayoutView:onShow');
            this._showDetails();
        },

        showBootstrappedDetails: function () {
            log('userFormLayoutView:showBootstrappedDetails');
            this.initializeRegions();
            this.$el = $('#content .user-form');
            this._showDetails();
        },

        _showDetails: function () {
            log('userFormLayoutView:_showDetails');
            var editAvatarView = new EditAvatarView({ el: '#avatar-fieldset', model: this.model });
            editAvatarView.render();
        },

        _contentChanged: function (e) {
            log('userFormLayoutView:_contentChanged');
            var target = $(e.currentTarget);
            var data = {};
            data[target.attr('id')] = target.attr('value');
            this.model.set(data);
        },

        _cancel: function () {
            app.showPreviousContentView();
        },

        _save: function () {
            this.model.save();
            app.showPreviousContentView();
        }
    });

    return UserFormLayoutView;
});