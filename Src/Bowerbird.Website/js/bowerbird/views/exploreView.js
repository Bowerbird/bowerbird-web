
window.Bowerbird.Views.ExploreView = Backbone.View.extend({
    tagName: 'section',

    className: 'triple-2',

    id: 'explore-list',

    events: {
        "click #explore-load-more-button": "loadNextExploreItems"
    },

    template: $.template('exploreTemplate', $('#explore-template')),

    loadingTemplate: $.template('exploreLoadingTemplate', $('#explore-loading-template')),

    loadMoreTemplate: $.template('exploreLoadMoreTemplate', $('#explore-load-more-template')),

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
        var exploreItemView = new Bowerbird.Views.ExploreItemView({ exploreItem: exploreItem });
        this.exploreItemViews.push(exploreItemView);
        this.toggleNoExploreItemsStatus(collection);
        if (options.index === 0) {
            $('#explore-items').prepend(exploreItemView.render().el);
        } else {
            $('#explore-items').append(exploreItemView.render().el);
        }
    },

    showNewExplore: function (stream) {
        $('#explore-items').empty();
        $('#explore-load-more').remove();
        if (explore.get('context') instanceof Bowerbird.Models.Team) {
        } else if (explore.get('context') instanceof Bowerbird.Models.Project) {
        } else if (explore.get('context') instanceof Bowerbird.Models.Organisation) {
        } else {
            // we're in the whole of bowerbird so possibly observations...?
        }
    },

    onExploreLoadingStart: function (explore) {
        $('#explore-load-more').remove();
        $('#explore-items').append($.tmpl('exploreLoadingTemplate', { text: 'Loading', showLoader: true }));
    },

    onExploreLoadingComplete: function (explore, collection) {
        this.toggleNoExploreItemsStatus(collection);
        if (collection.pageInfo().next) {
            $('#explore-list > div').append($.tmpl('exploreLoadMoreTemplate'));
        }
    },

    toggleNoExploreItemsStatus: function (collection) {
        $('#explore-status').remove();
        if (collection.length === 0) {
            $('#explore-items').append($.tmpl('exploreLoadingTemplate', { text: 'Nothing to explore! Create an Organisation, Team or Project.', showLoader: false }));
        }
    },

    loadNextExploreItems: function () {
        app.explore.setNextPage();
    }
});