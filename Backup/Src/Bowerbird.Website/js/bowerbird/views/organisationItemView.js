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

    var OrganisationItemView = Backbone.Marionette.ItemView.extend({

        className: 'organisation-details',

        template: 'OrganisationTileDetails',

        events: {
            'click .view-button': 'showItem',
            'click h3 a': 'showItem',
            'click .join-button': 'joinOrganisation',
            'click .leave-button': 'leaveOrganisation'
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

            if (app.authenticatedUser && this.model.id !== app.authenticatedUser.user.id) {
                this.showJoinButton(this.model.get('IsMember') === true ? 'leave' : 'join');
            }
        },

        refresh: function () {
        },

        showItem: function (e) {
            e.preventDefault();
            this.$el.find('.actions a').tipsy.revalidate();
            Backbone.history.navigate($(e.currentTarget).attr('href'), { trigger: true });
        },

        showJoinButton: function (type) {
            if (type === 'join') {
                var model = { JoinOrganisation: true };
                if (this.$el.find('.join-button, .leave-button').length === 0) {
                    this.$el.find('.actions').append(ich.Buttons(model));
                } else {
                    this.$el.find('.leave-button').replaceWith(ich.Buttons(model));
                }
                this.$el.find('.join-button').tipsy({ gravity: 's', html: true });
                this.$el.find('.actions .button').tipsy.revalidate();
            } else {
                var model = { LeaveOrganisation: true };
                if (this.$el.find('.join-button, .leave-button').length === 0) {
                    this.$el.find('.actions').append(ich.Buttons(model));
                } else {
                    this.$el.find('.join-button').replaceWith(ich.Buttons(model));
                }
                this.$el.find('.leave-button').tipsy({ gravity: 's', html: true });
                this.$el.find('.actions .button').tipsy.revalidate();

                this.$el.find(".leave-button").mouseover(function () {
                    $(this).text('Leave');
                }).mouseout(function () {
                    $(this).text('Joined');
                });
            }
        },

        joinOrganisation: function (e) {
            e.preventDefault();
            app.vent.trigger('join-organisation', this.model);
            this.showJoinButton('leave');
        },

        leaveOrganisation: function (e) {
            e.preventDefault();
            app.vent.trigger('leave-organisation', this.model);
            this.showJoinButton('join');
        }
    });

    return OrganisationItemView;

});