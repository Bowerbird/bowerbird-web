/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ProjectFormLayoutView
// -------------------------

define(['jquery', 'underscore', 'backbone', 'app', 'ich', 'multiselect', 'loadimage', 'fileupload'], function ($, _, Backbone, app, ich, loadImage) {

    var ProjectFormLayoutView = Backbone.Marionette.Layout.extend({

        className: 'form single-medium project-form',

        template: 'ProjectForm',

        events: {
            'click #cancel': '_cancel',
            'click #save': '_save',
            'change input#name': '_contentChanged',
            'change textarea#description': '_contentChanged',
            'change input#website': '_contentChanged',
            'change #team-field input:checkbox': '_teamChanged'
        },

        initialize: function (options) {
            log('projectFormLayoutView:initialize');
            this.teams = options.teams;
            //this._showDetails();
            //this._initMediaUploader();
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
            this._initMediaUploader();
        },

        showBootstrappedDetails: function () {
            log('projectFormLayoutView:showBootstrappedDetails');
            this.initializeRegions();
            this._showDetails();
        },

        _showDetails: function () {
            log('projectFormLayoutView:showDetails');
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

        _initMediaUploader: function () {
            log('projectFormLayoutView:initMediaUploader');
            $('#fileupload').fileupload({
                dataType: 'json',
                paramName: 'file',
                url: '/mediaresource/avatarupload',
                add: this._onUploadAdd,
                submit: this._onSubmitUpload,
                done: this._onUploadDone,
                limitConcurrentUploads: 1
            });
        },

        _onUploadAdd: function (e, data) {
            log('projectFormLayoutView:onUploadAdd');
            var self = this;
            var tempImage = loadImage(
                data.files[0],
                function (img) {
                    if (img.type === "error") {
                        log('Error loading image', img);
                    } else {
                        this._showTempMedia(img);
                    }
                },
                { maxHeight: 220 }
            );

            if (!tempImage) {
                alert('No support for file/blob API!');
            }

            data.submit();
        },

        _onSubmitUpload: function (e, data) {
            log('projectFormLayoutView:onSubmitUpload');
            data.formData = { Key: this.currentUploadKey, OriginalFileName: data.files[0].name };
        },

        _onUploadDone: function (e, data) {
            log('projectFormLayoutView:onUploadDone');
            var mediaResource = this.model.mediaResources.find(function (item) {
                return item.get('Key') === data.result.Key;
            });
            mediaResource.set(data.result);
            this._showUploadedMedia(mediaResource);
        },

        _showTempMedia: function (img) {
            log('projectFormLayoutView:showTempMedia');
            var $image = $(img);
            this.$el.find('#avatar-field img').replaceWith($image);
            this.$el.width($image.width());
            this.imageWidth = $image.width();
        },

        _showUploadedMedia: function (mediaResource) {
            log('projectFormLayoutView:showUploadedMedia');
            this.$el.find('#avatar-field img').replaceWith($('<img src="' + mediaResource.get('MediumImageUri') + '" alt="" />'));
        },

        _contentChanged: function (e) {
            log('projectFormLayoutView:contentChanged');
            var target = $(e.currentTarget);
            var data = {};
            data[target.attr('id')] = target.attr('value');
            this.model.set(data);
        },

        _teamChanged: function (e) {
            log('projectFormLayoutView:teamChanged');
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