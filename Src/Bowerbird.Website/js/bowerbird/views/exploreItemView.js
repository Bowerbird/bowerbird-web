
window.Bowerbird.Views.ExploreItemView = Backbone.View.extend({
    className: 'explore-item',

    initialize: function (options) {
        _.extend(this, Backbone.Events);
        this.ExploreItem = options.Group;
    },

    render: function () {
        var exploreItemJSON = this.ExploreItem.toJSON();
        switch (this.ExploreItem.get('Type')) {
            case 'organisation':
                var exploreItemHtml = ich.organisationItemTemplate(exploreItemJSON);
                this.$el.append(exploreItemHtml).addClass('organisation-explore-item');
                break;
            case 'team':
                var exploreItemHtml = ich.teamItemTemplate(exploreItemJSON);
                this.$el.append(exploreItemHtml).addClass('team-explore-item');
                break;
            case 'project':
                var exploreItemHtml = ich.projectItemTemplate(exploreItemJSON);
                this.$el.append(exploreItemHtml).addClass('project-explore-item');
                break;
            default:
                break;
        }
        return this;
    }
});