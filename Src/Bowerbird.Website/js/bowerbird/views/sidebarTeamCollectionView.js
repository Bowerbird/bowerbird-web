/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// SidebarTeamCollectionView
// -------------------------

// A collection of links in the sidebar
define(['jquery', 'underscore', 'backbone', 'app', 'views/sidebarteamitemview'], function ($, _, Backbone, app, SidebarTeamItemView) {

    var SidebarTeamCollectionView = Backbone.Marionette.CompositeView.extend({
        id: 'team-menu-group',

        className: 'menu-group',

        itemView: SidebarTeamItemView,

        template: 'SidebarMenuGroup',

        appendHtml: function (collectionView, itemView) {
            collectionView.$el.find('.menu-group-items > ul').append(itemView.el);
        },

        serializeData: function () {
            return {
                Model: {
                    Name: 'team',
                    Label: 'Teams'
                }
            };
        }
    });

    return SidebarTeamCollectionView;

});