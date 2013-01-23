///// <reference path="../../libs/log.js" />
///// <reference path="../../libs/require/require.js" />
///// <reference path="../../libs/jquery/jquery-1.7.2.js" />
///// <reference path="../../libs/underscore/underscore.js" />
///// <reference path="../../libs/backbone/backbone.js" />
///// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// OrganisationItemView
// ---------------

define(['jquery', 'underscore', 'backbone', 'ich', 'app', 'moment', 'timeago', 'tipsy'],
function ($, _, Backbone, ich, app, moment) {

    var leaveButtonTemplate = '<a href="#" class="leave-organisation-button button" title="&lt;b>Leave this organisation&lt;/b> &lt;br />Leave the organisation as a member. You will not longer see organisation activity in your timeline.">Leave</a>';
    var joinButtonTemplate = '<a href="#" class="join-organisation-button button" title="&lt;b>Join this organisation&lt;/b> &lt;br />Become a member of this organisation to add sightings and see organisation activity in your timelines.">Join</a>';

    var OrganisationItemView = Backbone.Marionette.ItemView.extend({

        className: 'organisation-details',

        template: 'OrganisationTileDetails',

        events: {
            'click .view-button': 'showItem',
            'click h3 a': 'showItem',
            'click .join-organisation-button': 'joinOrganisation',
            'click .leave-organisation-button': 'leaveOrganisation'
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
                if (app.authenticatedUser.organisations.get(this.model.id)) {
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

        joinOrganisation: function (e) {
            e.preventDefault();
            app.vent.trigger('joinOrganisation', this.model);
            this.$el.find('.actions .button').tipsy.revalidate();
            this.$el.find('.join-organisation-button').replaceWith(leaveButtonTemplate);
            this.$el.find('.join-organisation-button').tipsy({ gravity: 's', html: true });
        },

        leaveOrganisation: function (e) {
            e.preventDefault();
            app.vent.trigger('leaveOrganisation', this.model);
            this.$el.find('.actions .button').tipsy.revalidate();
            this.$el.find('.leave-organisation-button').replaceWith(joinButtonTemplate);
            this.$el.find('.leave-organisation-button').tipsy({ gravity: 's', html: true });
        }
    });

    return OrganisationItemView;

});