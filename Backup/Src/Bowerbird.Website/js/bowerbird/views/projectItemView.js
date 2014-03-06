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

    var ProjectItemView = Backbone.Marionette.ItemView.extend({

        className: 'project-details',

        template: 'ProjectTileDetails',

        events: {
            'click .view-button': 'showItem',
            'click h3 a': 'showItem',
            'click .join-button': 'joinProject',
            'click .leave-button': 'leaveProject'
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
                var model = { JoinProject: true };
                if (this.$el.find('.join-button, .leave-button').length === 0) {
                    this.$el.find('.actions').append(ich.Buttons(model));
                } else {
                    this.$el.find('.leave-button').replaceWith(ich.Buttons(model));
                }
                this.$el.find('.join-button').tipsy({ gravity: 's', html: true });
                this.$el.find('.actions .button').tipsy.revalidate();
            } else {
                var model = { LeaveProject: true };
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

        joinProject: function (e) {
            e.preventDefault();
            app.vent.trigger('join-project', this.model);
            this.showJoinButton('leave');
        },

        leaveProject: function (e) {
            e.preventDefault();
            app.vent.trigger('leave-project', this.model);
            this.showJoinButton('join');
        }
    });

    return ProjectItemView;

});