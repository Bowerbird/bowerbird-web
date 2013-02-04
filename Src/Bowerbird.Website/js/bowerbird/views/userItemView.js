///// <reference path="../../libs/log.js" />
///// <reference path="../../libs/require/require.js" />
///// <reference path="../../libs/jquery/jquery-1.7.2.js" />
///// <reference path="../../libs/underscore/underscore.js" />
///// <reference path="../../libs/backbone/backbone.js" />
///// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// UserItemView
// ---------------

define(['jquery', 'underscore', 'backbone', 'ich', 'app', 'moment', 'timeago', 'tipsy'],
function ($, _, Backbone, ich, app, moment) {

    var UserItemView = Backbone.Marionette.ItemView.extend({

        className: 'user-details',

        template: 'UserTileDetails',

        events: {
            'click .view-button': 'showItem',
            'click h3 a': 'showItem',
            'click .follow-button': 'followUser',
            'click .unfollow-button': 'unfollowUser'
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
                this.showFollowButton(this.model.get('IsFollowing') === true ? 'unfollow' : 'follow');
            }
        },

        refresh: function () {
        },

        showItem: function (e) {
            e.preventDefault();
            this.$el.find('.actions a').tipsy.revalidate();
            Backbone.history.navigate($(e.currentTarget).attr('href'), { trigger: true });
        },

        showFollowButton: function (type) {
            if (type === 'follow') {
                var model = { Follow: true };
                if (this.$el.find('.follow-button, .unfollow-button').length === 0) {
                    this.$el.find('.actions').append(ich.Buttons(model));
                } else {
                    this.$el.find('.unfollow-button').replaceWith(ich.Buttons(model));
                }
                this.$el.find('.follow-button').tipsy({ gravity: 's', html: true });
                this.$el.find('.actions .button').tipsy.revalidate();
            } else {
                var model = { Unfollow: true };
                if (this.$el.find('.follow-button, .unfollow-button').length === 0) {
                    this.$el.find('.actions').append(ich.Buttons(model));
                } else {
                    this.$el.find('.follow-button').replaceWith(ich.Buttons(model));
                }
                this.$el.find('.unfollow-button').tipsy({ gravity: 's', html: true });
                this.$el.find('.actions .button').tipsy.revalidate();

                this.$el.find(".unfollow-button").mouseover(function () {
                    $(this).text('Unfollow');
                }).mouseout(function () {
                    $(this).text('Following');
                });
            }
        },

        followUser: function (e) {
            e.preventDefault();
            app.vent.trigger('follow-user', this.model);
            this.showFollowButton('unfollow');
        },

        unfollowUser: function (e) {
            e.preventDefault();
            app.vent.trigger('unfollow-user', this.model);
            this.showFollowButton('follow');
        }
    });

    return UserItemView;

});