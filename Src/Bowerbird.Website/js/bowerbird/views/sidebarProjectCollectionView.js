/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// SidebarCollectionView
// ---------------------

// A collection of links in the sidebar
define(['jquery', 'underscore', 'backbone', 'app', 'views/sidebarprojectitemview'], function ($, _, Backbone, app, SidebarProjectItemView) {

    var SidebarProjectCollectionView = Backbone.Marionette.CollectionView.extend({
        itemView: SidebarProjectItemView,

        appendHtml: function (collectionView, itemView) {
            collectionView.$el.find('.menu-group-options').before(itemView.el);
        }
    });

    return SidebarProjectCollectionView;

});