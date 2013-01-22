/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/jquery/jquery.fileupload.js" />
/// <reference path="../../libs/jquery/load-image.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// OrganisationFormView
// ---------------

define(['jquery', 'underscore', 'backbone', 'app', 'ich', 'loadimage', 'views/editavatarview', 'views/backgroundimageformview', 'fileupload', 'multiselect'],
function ($, _, Backbone, app, ich, loadImage, AvatarImageFormView, BackgroundImageFormView) {

    var OrganisationFormView = Backbone.Marionette.Layout.extend({
        viewType: 'form',

        className: 'form form-medium single organisation-form',

        template: 'OrganisationForm',

        regions: {
            avatar: '#avatar-fieldset',
            backgroundRegion: '#background-fieldset'
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
                    Organisation: this.model.toJSON()
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
            var avatarImageFormView = new AvatarImageFormView({ el: '.avatar-field', model: this.model });
            avatarImageFormView.render();

            var backgroundImageFormView = new BackgroundImageFormView({ el: '.background-field', model: this.model });
            backgroundImageFormView.render();
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
                this.model.set('Id', this.model.id.replace('organisations/', ''));
            }

            this.model.save();
            app.showPreviousContentView();
        }
    });

    return OrganisationFormView;
});