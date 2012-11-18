/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// HeaderView
// ----------

// The app's header
define(['jquery', 'underscore', 'backbone', 'app'],
function ($, _, Backbone, app) {

    var HeaderView = Backbone.Marionette.ItemView.extend({
        
        template: 'Header',

        events: {
            'click .user-menu-item': 'showMenu'
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
            this.$el.find('#explore-menu a, #account-menu .view-your-profile-button, .login-button, .register-button').on('click', function (e) {
                e.preventDefault();
                Backbone.history.navigate($(this).attr('href'), { trigger: true });
                return false;
            });
        },

        showMenu: function (e) {
            $('.sub-menu').removeClass('active');
            this.$el.find('.sub-menu').addClass('active');
            e.stopPropagation();
        }
    });

    app.addInitializer(function () {
        $(function () {
            // Bootstrap header on initial load from server
            if (app.isPrerendering()) {
                var headerType = app.isPrerendering('home') && app.authenticatedUser == null ? 'home' : 'default';
                var headerView = new HeaderView({ el: 'header', headerType: headerType });
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
            });
        });
    });

    return HeaderView;

});