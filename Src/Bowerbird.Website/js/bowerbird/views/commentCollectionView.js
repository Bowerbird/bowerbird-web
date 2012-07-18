/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// CommentCollectionView
// ---------------------

// Shows Explore organisation items
define(['jquery','underscore','backbone','app','ich','views/organisationitemview'],
function ($, _, Backbone, app, ich, OrganisationItemView) 
{
    var CommentCollectionView = Backbone.Marionette.CollectionView.extend({

        tagName: 'ul',

        className: 'comments',

        itemView: CommentItemView

    });

    return CommentCollectionView;
});