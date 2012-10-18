/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ProjectExploreView
// ------------------

define(['jquery','underscore','backbone','app','ich','views/projectitemview'],
function ($, _, Backbone, app, ich, ProjectItemView) 
{
    var ProjectExploreView = Backbone.Marionette.CompositeView.extend({
        viewType: 'details',
        
        className: 'projects',

        template: 'ProjectList',

        itemView: ProjectItemView,

        appendHtml: function (collectionView, itemView) {
            var items = this.collection.pluck('Id');
            var index = _.indexOf(items, itemView.model.id);

            var $li = collectionView.$el.find('.project-list > ul > li:eq(' + (index) + ')');

            if ($li.length === 0) {
                collectionView.$el.find('.project-list > ul').append(itemView.el);
            } else {
                $li.before(itemView.el);
            }
        },

        showBootstrappedDetails: function () {
            this.$el = $('#content .projects');
        }
    });

    return ProjectExploreView;
});