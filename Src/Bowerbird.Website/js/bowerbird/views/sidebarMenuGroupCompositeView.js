/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// SidebarMenuGroupCompositeView
// -----------------------------

define(['jquery', 'underscore', 'backbone', 'app'], function ($, _, Backbone, app) {

    var SidebarMenuGroupCompositeView = Backbone.Marionette.CompositeView.extend({
        className: 'menu-group',

        template: 'SidebarMenuGroup',

        initialize: function (options) {
            this.type = options.type;
            this.label = options.label;
        },

        appendHtml: function (collectionView, itemView) {
            var index = collectionView.collection.indexOf(itemView.model);
            if (index === 0 && collectionView.$el.find('#' + this.type + '-menu-group-list > li').length === 0) {
                collectionView.$el.find('#' + this.type + '-menu-group-list').append(itemView.el);
            } else {
                collectionView.$el.find('#' + this.type + '-menu-group-list > li').eq(index - 1).after(itemView.el);
            }
        },

        serializeData: function () {
            return {
                Model: {
                    Name: this.type,
                    Label: this.label
                }
            };
        }
    });

    return SidebarMenuGroupCompositeView;

});