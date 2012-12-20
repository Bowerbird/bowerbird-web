///// <reference path="../../libs/log.js" />
///// <reference path="../../libs/require/require.js" />
///// <reference path="../../libs/jquery/jquery-1.7.2.js" />
///// <reference path="../../libs/underscore/underscore.js" />
///// <reference path="../../libs/backbone/backbone.js" />
///// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ProjectItemView
// ---------------

define(['jquery', 'underscore', 'backbone', 'ich', 'app', 'moment', 'timeago', 'tipsy'],
function ($, _, Backbone, ich, app, moment) {

    var leaveButtonTemplate = '<a href="#" class="leave-project-button button" title="&lt;b>Leave this project&lt;/b> &lt;br />Leave the project as a member. You will not longer see project activity in your timeline.">Leave</a>';
    var joinButtonTemplate = '<a href="#" class="join-project-button button" title="&lt;b>Join this project&lt;/b> &lt;br />Become a member of this project to add sightings and see project activity in your timelines.">Join</a>';

    var ProjectItemView = Backbone.Marionette.ItemView.extend({

        className: 'project-details',

        template: 'ProjectTileDetails',

        events: {
            'click .view-button': 'showItem',
            'click h3 a': 'showItem',
            'click .join-project-button': 'joinProject',
            'click .leave-project-button': 'leaveProject'
        },

        initialize: function (options) {
            _.bindAll(this, 'refresh');
        },

        serializeData: function () {
            var viewModel = this.model.toJSON();
            return viewModel;
        },

        onShow: function () {
            this._showDetails();
        },

        onRender: function () {
            this._showDetails();
        },

        showBootstrappedDetails: function () {
            this._showDetails();
        },

        _showDetails: function () {
            var resizeTimer;
            var refresh = this.refresh;
            $(window).resize(function () {
                clearTimeout(resizeTimer);
                resizeTimer = setTimeout(function () {
                    refresh();
                }, 100);
            });

            if (app.authenticatedUser) {
                if (app.authenticatedUser.projects.get(this.model.id)) {
                    this.$el.find('.actions').prepend(leaveButtonTemplate);
                } else {
                    this.$el.find('.actions').prepend(joinButtonTemplate);
                }
            }

            this.$el.find('.actions .button').tipsy({ gravity: 's', html: true });
        },

        refresh: function () {
        },

        showItem: function (e) {
            e.preventDefault();
            this.$el.find('.actions a').tipsy.revalidate();
            Backbone.history.navigate($(e.currentTarget).attr('href'), { trigger: true });
        },

        joinProject: function (e) {
            e.preventDefault();
            app.vent.trigger('joinProject', this.model);
            this.$el.find('.actions .button').tipsy.revalidate();
            this.$el.find('.join-project-button').replaceWith(leaveButtonTemplate);
            this.$el.find('.join-project-button').tipsy({ gravity: 's', html: true });
        },

        leaveProject: function (e) {
            e.preventDefault();
            app.vent.trigger('leaveProject', this.model);
            this.$el.find('.actions .button').tipsy.revalidate();
            this.$el.find('.leave-project-button').replaceWith(joinButtonTemplate);
            this.$el.find('.leave-project-button').tipsy({ gravity: 's', html: true });
        }
    });

    return ProjectItemView;

});