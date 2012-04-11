
window.Bowerbird.Models.Explore = Backbone.Model.extend({
    defaults: {
        context: null, // string containing either 'projects', 'teams', or 'organisations'
        filter: null,
        uri: ''
    },

    initialize: function (options) {
        _.extend(this, Backbone.Events);
        _.bindAll(this);
        this.groups = new Bowerbird.Collections.Groups();
    },

    isSet: function () {
        return this.get('filter') != null && this.get('uri') != null;
    },

    setNewExplore: function (exploreContext) {
        this.set('context', exploreContext);
        var uri = 'members/' + this.get('context') + '/list';
        this.set('uri', uri);
        this.trigger('newExplore', this);
        this.trigger('fetchingItemsStarted', this);
        this.groups.fetchFirstPage(this);
    },

    setNewFilter: function (filter) {
        this.set('filter', filter);
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