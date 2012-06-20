/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// TeamCollectionView
// ------------------

// Shows Explore project items for selected Project
define(['jquery','underscore','backbone','app','ich','views/teamitemview'],
function ($, _, Backbone, app, ich, TeamItemView) 
{
    var TeamCollectionView = Backbone.Marionette.CollectionView.extend({

        tagName: 'ul',

        className: 'explore-groups',

        itemView: TeamItemView

    });

    return TeamCollectionView;
});