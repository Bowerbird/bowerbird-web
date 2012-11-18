/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// AccountController & AccountRouter
// ---------------------------------

define(['jquery', 'underscore', 'backbone', 'app', 'views/accountloginview', 'views/accountregisterview'],
function ($, _, Backbone, app, AccountLoginView, AccountRegisterView) {

    var AccountRouter = Backbone.Marionette.AppRouter.extend({
        appRoutes: {
            'account/login': 'showLogin',
            'account/register': 'showRegister'
        }
    });

    var AccountController = {};

    var getModel = function (uri, action, data) {
        var deferred = new $.Deferred();
        if (app.isPrerenderingView('account')) {
            deferred.resolve(app.prerenderedView.data);
        } else {
            $.ajax({
                url: uri,
                type: action || 'GET',
                data: data
            }).done(function (data) {
                deferred.resolve(data.Model);
            });
        }
        return deferred.promise();
    };

    // Public API
    // ----------

    AccountController.showLogin = function () {
        if (app.authenticatedUser) {
            window.location.replace('/');
            return;
        }
        var options = {};
        if (app.isPrerenderingView('account')) {
            options['el'] = '.login';
        }
        var accountLoginView = new AccountLoginView(options);

        app.showContentView('Login', accountLoginView, 'account');
    };

    AccountController.showRegister = function () {
        if (app.authenticatedUser) {
            window.location.replace('/');
            return;
        }
        
        var options = {};
        if (app.isPrerenderingView('account')) {
            options['el'] = '.register';
        }
        var accountRegisterView = new AccountRegisterView(options);

        app.showContentView('Join', accountRegisterView, 'account');
    };

    // Event Handlers
    // --------------

    app.vent.on('close-call-to-action', function (name) {
        $.when(getModel('/account/closecalltoaction', 'POST', { name: name }));
    });

    app.addInitializer(function () {
        this.accountRouter = new AccountRouter({
            controller: AccountController
        });
    });

    return AccountController;

});