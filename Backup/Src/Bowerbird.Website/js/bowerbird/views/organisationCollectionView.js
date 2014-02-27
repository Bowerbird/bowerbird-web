/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// OrganisationCollectionView
// --------------------------

// Shows Explore organisation items
define(['jquery','underscore','backbone','app','ich','views/organisationitemview'],
function ($, _, Backbone, app, ich, OrganisationItemView) 
{
    var OrganisationCollectionView = Backbone.Marionette.CollectionView.extend({

        tagName: 'ul',

        className: 'explore-groups',

        itemView: OrganisationItemView

    });

    return OrganisationCollectionView;
});