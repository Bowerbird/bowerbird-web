/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// HeaderView
// ----------

// The app's header
define(['jquery', 'underscore', 'backbone', 'app', 'carousel'],
function ($, _, Backbone, app) {

    var HeaderView = Backbone.Marionette.ItemView.extend({

        template: 'Header',

        events: {
            'click .user-menu-item': 'showMenu',
            'click #explore-menu a': 'selectMenuItem',
            'click .sub-menu .view-your-profile-button': 'selectMenuItem',
            'click .sub-menu .change-password-button': 'selectMenuItem',
            'click .sub-menu .account-update-button': 'selectMenuItem',
            'click .login-button': 'selectMenuItem',
            'click .register-button': 'selectMenuItem',
            'click .banner a': 'selectMenuItem'
        },

        initialize: function (options) {
            this.headerType = options.headerType;
        },

        serializeData: function () {
            var viewModel = {
                Model: {
                    HomeHeader: this.headerType === 'home'
                }
            };

            if (app.authenticatedUser) {
                viewModel.Model.AuthenticatedUser = {
                    User: app.authenticatedUser.user.toJSON()
                };
            }

            return viewModel;
        },

        onShow: function () {
            this._showDetails();
        },

        showBootstrappedDetails: function () {
            this._showDetails();
        },

        _showDetails: function () {
            var dur = 1000;
            var pDur = 3000;
            var that = this;

            this.carousel = this.$el.find('#header-carousel ul').carouFredSel({
                width: "50%",
                height: '27em',
                items: {
                    visible: 1,
                    width: '640px',
                    height: '480px'
                },
                direction: "up",
                auto: {
                    fx: 'fade',
                    easing: 'linear',
                    duration: dur,
                    timeoutDuration: pDur,
                    onBefore: function (data) {
                        that.animate(data.items.visible, pDur + (dur * 3));
                    },
                    onAfter: function (data) {
                        data.items.old.find('img').stop().css({
                            width: 640,
                            height: 480,
                            marginTop: 0,
                            marginLeft: 0
                        });
                    }
                },

                onCreate: function (data) {
                    that.animate(data.items, pDur + (dur * 2));
                }
            });
        },

        animate: function (item, dur) {
            var obj = {
                width: 800,
                height: 600
            };
            switch (Math.ceil(Math.random() * 2)) {
                case 1:
                    obj.marginTop = 0;
                    break;
                case 2:
                    obj.marginTop = -120;
                    break;
            }
            switch (Math.ceil(Math.random() * 2)) {
                case 1:
                    obj.marginLeft = 0;
                    break;
                case 2:
                    obj.marginLeft = -200;
                    break;
            }
            item.find('img').animate(obj, dur, 'linear');
        },

        showMenu: function (e) {
            app.vent.trigger('close-sub-menus');
            this.$el.find('.sub-menu').addClass('active');
            e.stopPropagation();
        },

        selectMenuItem: function (e) {
            e.preventDefault();
            app.vent.trigger('close-sub-menus');
            Backbone.history.navigate($(e.currentTarget).attr('href'), { trigger: true });
            return false;
        },

        beforeClose: function () {
            this.carousel.stop(true);
        }
    });

    app.addInitializer(function () {
        $(function () {
            // Bootstrap header on initial load from server
            if (app.isPrerendering()) {
                var headerType = app.isPrerendering('home') && app.authenticatedUser == null ? 'home' : 'default';
                var headerView = new HeaderView({ el: $('header'), headerType: headerType });
                app.header.attachView(headerView);
                headerView.showBootstrappedDetails();
            }

            // Subscribe to history to get the name of the route to filter header types
            Backbone.history.on('route', function (history, name, args) {
                var headerType = name === 'showHome' && app.authenticatedUser == null ? 'home' : 'default';

                if (headerType !== app.header.currentView.headerType) {
                    var headerView = new HeaderView({ headerType: headerType });
                    app.header.show(headerView);
                }
            }, this);
        });
    });

    return HeaderView;

});