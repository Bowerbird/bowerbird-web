/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// OrganisationFormLayoutView
// --------------------------

define(['jquery', 'underscore', 'backbone', 'app', 'ich', 'loadimage', 'views/editavatarview', 'fileupload', 'multiselect'], function ($, _, Backbone, app, ich, loadImage, EditAvatarView) {

    var OrganisationFormLayoutView = Backbone.Marionette.Layout.extend({

        className: 'form single-medium organisation-form',

        tempalte: 'OrganisationForm',

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
            log('organisationFormLayoutView.initialize');
        },

        serializeData: function () {
            log('organisationFormLayoutView:serializeData');
            return {
                Model: {
                    Organisation: this.model.toJSON()
                }
            };
        },

        onShow: function () {
            log('organisationFormLayoutView:onShow');
            this._showDetails();
        },

        showBootstrappedDetails: function () {
            log('organisationFormLayoutView:showBootstrappedDetails');
            this.initializeRegions();
            this.$el = $('#content .organisation-form');
            this._showDetails();
        },

        _showDetails: function () {
            log('organisationFormLayoutView:_showDetails');
            var editAvatarView = new EditAvatarView({ el: '#avatar-fieldset', model: this.model });
            editAvatarView.render();
        },

        _contentChanged: function (e) {
            log('organisationFormLayoutView:_contentChanged');
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
        }
    });

    return OrganisationFormLayoutView;
});