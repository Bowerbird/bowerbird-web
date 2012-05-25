/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ProjectFormLayoutView
// -------------------------

define(['jquery', 'underscore', 'backbone', 'app', 'ich', 'loadimage', 'views/editavatarview', 'fileupload', 'multiselect'], function ($, _, Backbone, app, ich, loadImage, EditAvatarView) {

    var OrganisationFormLayoutView = Backbone.Marionette.Layout.extend({

        className: 'form single-medium organisation-create-form',

        tempalte: 'OrganisationForm',

        regions: {
            avatar: '#avatar-fieldset'
        },

        events: {
            'click #cancel': '_cancel',
            'click #save': '_save',
            'change input#name': '_contentChanged',
            'change textarea#description': '_contentChanged',
            'change input#website': '_contentChanged'
        },

        serializeData: function () {
            return {
                Model: {
                    Organisation: this.model.toJSON()
                }
            };
        },

        onShow: function () {
            this._showDetails();
        },

        showBootstrappedDetails: function () {
            this.initializeRegions();
        },

        _showDetails: function () {
            log('projectFormLayoutView:showDetails');
            var editAvatarView = new EditAvatarView({ el: '#avatar-fieldset', model: this.model });
            editAvatarView.render();
        },

        _contentChanged: function (e) {
            var target = $(e.currentTarget);
            var data = {};
            data[target.attr('id')] = target.attr('value');
            this.model.set(data);
        },

        _cancel: function () {
        },

        _save: function () {
            this.model.save();
        }
    });

    return OrganisationFormLayoutView;
});