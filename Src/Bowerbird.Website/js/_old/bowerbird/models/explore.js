
window.Bowerbird.Models.Explore = Backbone.Model.extend({
    defaults: {
        Context: null, // string containing either 'projects', 'teams', or 'organisations'
        Filter: null,
        Uri: ''
    },

    initialize: function (options) {
        _.extend(this, Backbone.Events);
        _.bindAll(this);
        this.groups = new Bowerbird.Collections.Groups();
    },

    isSet: function () {
        return this.get('Filter') != null && this.get('Uri') != null;
    },

    setNewExplore: function (exploreContext) {
        this.set('Context', exploreContext);
        var uri = this.get('Context') + '/list';
        this.set('Uri', uri);
        this.trigger('newExplore', this);
        this.trigger('fetchingItemsStarted', this);
        this.groups.fetchFirstPage(this);
    },

    setNewFilter: function (filter) {
        this.set('Filter', filter);
        this.trigger('newExploreFilter', this);
        this.trigger('fetchingItemsStarted', this);
        this.groups.fetchFirstPage(this);
    },

    setNextPage: function () {
        this.trigger('newExplorePage', this);
        this.trigger('fetchingItemsStarted', this);
        this.groups.fetchNextPage(this);
    }
});