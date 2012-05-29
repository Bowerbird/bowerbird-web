/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/jquery/jquery.fileupload.js" />
/// <reference path="../../libs/jquery/load-image.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// PostFormLayoutView
// -------------------------

define(['jquery', 'underscore', 'backbone', 'app', 'ich'], function ($, _, Backbone, app, ich) {

    var PostFormLayoutView = Backbone.Marionette.Layout.extend({

        className: 'form single-medium post-form',

        template: 'PostForm',

        events: {
            'click #cancel': '_cancel',
            'click #save': '_save',
            'change input#Subject': '_contentChanged',
            'change textarea#Message': '_contentChanged'
        },

        initialize: function (options) {
            log('postFormLayoutView:initialize');
        },

        serializeData: function () {
            log('postFormLayoutView:serializeData');
            return {
                Model: {
                    Post: this.model.toJSON()
                }
            };
        },

        onShow: function () {
            log('postFormLayoutView:onShow');
            this._showDetails();
        },

        showBootstrappedDetails: function () {
            log('postFormLayoutView:showBootstrappedDetails');
            this.initializeRegions();
            this._showDetails();
        },

        _showDetails: function () {
            log('postFormLayoutView:_showDetails');
        },

        _contentChanged: function (e) {
            log('postFormLayoutView:_contentChanged');
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

    return PostFormLayoutView;
});