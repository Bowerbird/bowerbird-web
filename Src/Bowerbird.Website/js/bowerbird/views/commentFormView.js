/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// CommentFormView
// ---------------

define(['jquery', 'underscore', 'backbone', 'app', 'ich'],
function ($, _, Backbone, app, ich) {
    var CommentFormView = Backbone.Marionette.ItemView.extend({

        id: 'comment-form',

        template: 'CommentForm',

        events: {
            'click .cancel-add-comment-button': '_cancel',
            'click .save-comment-button': '_save',
            'change .add-comment-text': '_onCommentChanged'
        },

        serializeData: function () {
            return {
                Model: this.model.toJSON()
            };
        },

        initialize: function (options) {

        },

        onRender: function () {

        },

        _onCommentChanged: function () {
            this.model.set('Message', $('.add-comment-text').val());
        },

        _cancel: function () {
            this.remove();
        },

        _save: function () {
            this.model.save();
        }
    });

    return CommentFormView;
});