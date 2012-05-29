/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// SidebarOrganisationCollectionView
// ---------------------------------

// A collection of links in the sidebar
define(['jquery', 'underscore', 'backbone', 'app', 'views/sidebarorganisationitemview'], function ($, _, Backbone, app, SidebarOrganisationItemView) {

    var SidebarOrganisationCollectionView = Backbone.Marionette.CollectionView.extend({
        itemView: SidebarOrganisationItemView,

        appendHtml: function (collectionView, itemView) {
            collectionView.$el.find('.menu-group-options').before(itemView.el);
        }
    });

    return SidebarOrganisationCollectionView;

});