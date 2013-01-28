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

define(['jquery', 'underscore', 'backbone', 'app', 'ich', 'loadimage', 'views/editavatarview', 'views/backgroundimageformview', 'fileupload', 'multiselect'],
function ($, _, Backbone, app, ich, loadImage, AvatarImageFormView, BackgroundImageFormView) {

    var ProjectFormView = Backbone.Marionette.Layout.extend({
        viewType: 'form',

        className: 'form form-medium single project-form',

        template: 'ProjectForm',

        regions: {
            //avatar: '#avatar-fieldset',
            backgroundRegion: '#background-fieldset'
        },

        events: {
            'click #cancel': '_cancel',
            'click #save': '_save',
            'change input#Name': '_contentChanged',
            'change textarea#Description': '_contentChanged',
            'change input#Website': '_contentChanged',
            'change #categories-field input:checkbox': '_categoriesChanged'
        },

        initialize: function (options) {
            if (this.model.id) {
                this.viewEditMode = 'update';
            } else {
                this.viewEditMode = 'create';
            }
            this.categoriesSelectList = options.categoriesSelectList;
        },

        serializeData: function () {
            return {
                Model: {
                    Project: this.model.toJSON(),
                    CategoriesSelectList: this.categoriesSelectList
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

            this.categoriesListSelectView = this.$el.find('#Categories').multiSelect({
                selectAll: false,
                listHeight: 260,
                messageText: 'Select more than one category if required',
                noOptionsText: 'No Categories',
                noneSelected: '<span class="default-option">Select Categories</span>',
                oneOrMoreSelected: function (selectedOptions) {
                    var $selectedHtml = $('<span />');
                    var cats = [];
                    _.each(selectedOptions, function (option) {
                        cats.push(option.text);
                    });
                    $selectedHtml.append(cats.join(', '));
                    return $selectedHtml;
                }
            });
        },

        _contentChanged: function (e) {
            var target = $(e.currentTarget);
            var data = {};
            data[target.attr('id')] = target.attr('value');
            this.model.set(data);
        },

        _categoriesChanged: function (e) {
            var $checkbox = $(e.currentTarget);
            if ($checkbox.attr('checked') === 'checked') {
                var category = $checkbox.attr('value');
                this.model.addCategory(category);
            } else {
                this.model.removeCategory($checkbox.attr('value'));
            }
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