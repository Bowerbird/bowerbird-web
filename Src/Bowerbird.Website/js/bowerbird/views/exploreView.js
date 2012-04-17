
window.Bowerbird.Views.ExploreView = Backbone.View.extend({
    tagName: 'section',

    className: 'triple-2',

    id: 'explore-list',

    events: {
        "click #explore-load-more-button": "loadNextExploreItems"
    },

    initialize: function (options) {
        _.extend(this, Backbone.Events);
        _.bindAll(this, 'addExploreItem');
        this.exploreItemViews = [];
        app.explore.groups.on('add', this.addGroup, this);
        app.explore.on('newExplore', this.showNewExplore, this);
        app.explore.on('fetchingItemsStarted', this.onExploreLoadingStart, this);
        app.explore.on('fetchingItemsComplete', this.onsExploreLoadingComplete, this);
    },

    render: function () {
        $.tmpl('exploreListTemplate').appendTo(this.$el);
        return this;
    },

    addExploreItem: function (exploreItem, collection, options) {
        var exploreItemView = new Bowerbird.Views.ExploreItemView({ ExploreItem: exploreItem });
        this.exploreItemViews.push(exploreItemView);
        this.toggleNoExploreItemsStatus(collection);
        if (options.Index === 0) {
            $('#explore-items').prepend(exploreItemView.render().el);
        } else {
            $('#explore-items').append(exploreItemView.render().el);
        }
    },

    showNewExplore: function (stream) {
        $('#explore-items').empty();
        $('#explore-load-more').remove();
        if (app.explore.get('Context') instanceof Bowerbird.Models.Team) {
        } else if (app.explore.get('Context') instanceof Bowerbird.Models.Project) {
        } else if (app.explore.get('context') instanceof Bowerbird.Models.Organisation) {
        } else {
            // we're in the whole of bowerbird so possibly observations...?
        }
    },

    onExploreLoadingStart: function (explore) {
        $('#explore-load-more').remove();
        $('#explore-items').append($.tmpl('exploreLoadingTemplate', { Text: 'Loading', ShowLoader: true }));
    },

    onExploreLoadingComplete: function (explore, collection) {
        this.toggleNoExploreItemsStatus(collection);
        if (collection.pageInfo().Next) {
            $('#explore-list > div').append($.tmpl('exploreLoadMoreTemplate'));
        }
    },

    toggleNoExploreItemsStatus: function (collection) {
        $('#explore-status').remove();
        if (collection.length === 0) {
            $('#explore-items').append($.tmpl('exploreLoadingTemplate', { Text: 'Nothing to explore! Create an Organisation, Team or Project.', ShowLoader: false }));
        }
    },

    loadNextExploreItems: function () {
        app.explore.setNextPage();
    }
});