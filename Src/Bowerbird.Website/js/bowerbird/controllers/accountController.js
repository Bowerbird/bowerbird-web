/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// AccountController & AccountRouter
// ---------------------------------

define(['jquery', 'underscore', 'backbone', 'app'],
function ($, _, Backbone, app, HomeLayoutView) {

    var AccountRouter = Backbone.Marionette.AppRouter.extend({
        appRoutes: {
            'account/login': 'showLogin',
            'account/register': 'showRegister'
        }
    });

    var AccountController = {};

    // Public API
    // ----------

    AccountController.showLogin = function () {
    };

    AccountController.showRegister = function () {
    };

    app.addInitializer(function () {
        this.accountRouter = new AccountRouter({
            controller: AccountController
        });
    });

    return AccountController;

});