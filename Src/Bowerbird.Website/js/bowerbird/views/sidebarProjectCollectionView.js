/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// SidebarProjectCollectionView
// ----------------------------

// A collection of links in the sidebar
define(['jquery', 'underscore', 'backbone', 'app', 'views/sidebarprojectitemview'], function ($, _, Backbone, app, SidebarProjectItemView) {

    var SidebarProjectCollectionView = Backbone.Marionette.CollectionView.extend({
        tagName: 'ul',

        id: 'project-menu-group-list',

        itemView: SidebarProjectItemView,

        appendHtml: function (collectionView, itemView) {
            collectionView.$el.append(itemView.el);
        }
    });

    return SidebarProjectCollectionView;

});