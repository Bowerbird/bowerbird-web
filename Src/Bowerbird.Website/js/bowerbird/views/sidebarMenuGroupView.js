/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// SidebarMenuGroupView
// --------------------

define(['jquery', 'underscore', 'backbone', 'app', 'tinyscroller'], function ($, _, Backbone, app) {

    var SidebarMenuGroupView = Backbone.Marionette.CompositeView.extend({
        className: 'menu-group',

        template: 'SidebarMenuGroup',

        initialize: function (options) {
            _.bindAll(this, 'onItemAdd', 'onItemRemove');

            this.type = options.type;
            this.label = options.label;

            this.collection.on('add', this.onItemAdd);
            this.collection.on('remove', this.onItemRemove);
        },

        onRender: function () {
            if (this.collection.length > 0) {
                this.$el.show();
            }

            var that = this;
            $(function () {
                that.$el.find('.scrollbar-container').tinyscrollbar();
            });
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
                    Label: this.label,
                    Icon: this.type === 'userproject' ? 'user' : this.type
                }
            };
        },

        onItemAdd: function (item) {
            this.$el.show();
        },

        onItemRemove: function (item) {
            if (this.collection.length === 0) {
                this.$el.hide();
            }
        }

    });

    return SidebarMenuGroupView;

});