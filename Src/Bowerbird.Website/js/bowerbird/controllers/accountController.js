﻿/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// AccountController & AccountRouter
// ---------------------------------

define(['jquery', 'underscore', 'backbone', 'app', 'models/user', 'views/accountloginview', 'views/accountregisterview', 'views/userformview'],
function ($, _, Backbone, app, User, AccountLoginView, AccountRegisterView, UserFormView) {

    var AccountRouter = Backbone.Marionette.AppRouter.extend({
        appRoutes: {
            'account/login': 'showLogin',
            'account/register': 'showRegister',
            'account/update': 'showUserUpdateForm'
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

    var docCookies = {
        getItem: function (sKey) {
            if (!sKey || !this.hasItem(sKey)) { return null; }
            return unescape(document.cookie.replace(new RegExp("(?:^|.*;\\s*)" + escape(sKey).replace(/[\-\.\+\*]/g, "\\$&") + "\\s*\\=\\s*((?:[^;](?!;))*[^;]?).*"), "$1"));
        },
        setItem: function (sKey, sValue, vEnd, sPath, sDomain, bSecure) {
            if (!sKey || /^(?:expires|max\-age|path|domain|secure)$/i.test(sKey)) { return; }
            var sExpires = "";
            if (vEnd) {
                switch (vEnd.constructor) {
                    case Number:
                        sExpires = vEnd === Infinity ? "; expires=Tue, 19 Jan 2038 03:14:07 GMT" : "; max-age=" + vEnd;
                        break;
                    case String:
                        sExpires = "; expires=" + vEnd;
                        break;
                    case Date:
                        sExpires = "; expires=" + vEnd.toGMTString();
                        break;
                }
            }
            document.cookie = escape(sKey) + "=" + escape(sValue) + sExpires + (sDomain ? "; domain=" + sDomain : "") + (sPath ? "; path=" + sPath : "") + (bSecure ? "; secure" : "");
        },
        removeItem: function (sKey, sPath) {
            if (!sKey || !this.hasItem(sKey)) { return; }
            document.cookie = escape(sKey) + "=; expires=Thu, 01 Jan 1970 00:00:00 GMT" + (sPath ? "; path=" + sPath : "");
        },
        hasItem: function (sKey) {
            return (new RegExp("(?:^|;\\s*)" + escape(sKey).replace(/[\-\.\+\*]/g, "\\$&") + "\\s*\\=")).test(document.cookie);
        },
        keys: /* optional method: you can safely remove it! */function () {
            var aKeys = document.cookie.replace(/((?:^|\s*;)[^\=]+)(?=;|$)|^\s*|\s*(?:\=[^;]*)?(?:\1|$)/g, "").split(/\s*(?:\=[^;]*)?;\s*/);
            for (var nIdx = 0; nIdx < aKeys.length; nIdx++) { aKeys[nIdx] = unescape(aKeys[nIdx]); }
            return aKeys;
        }
    };

    // Public API
    // ----------

    AccountController.showLogin = function () {
        if (app.authenticatedUser) {
            window.location.replace('/');
            return;
        }
        var options = {
            email: '',
            returnUrl: ''
        };
        if (app.isPrerenderingView('account')) {
            options['el'] = '.login';
        }

        if (app.isPrerendering()) {
            options.email = app.prerenderedView.data.AccountLogin.Email;
            options.returnUrl = app.prerenderedView.data.AccountLogin.ReturnUrl;
        } else {
            if (docCookies.hasItem('56277e138f774318ab152a84dad7adf9')) {
                options.email = docCookies.getItem('56277e138f774318ab152a84dad7adf9');
            }
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

    AccountController.showUserUpdateForm = function () {
        var options = {};
        if (app.isPrerenderingView('account')) {
            options['el'] = '.user-form';
        }

        if (app.isPrerenderingView('account')) {
            log('user edit', app.prerenderedView.data);
            options.model = new User(app.prerenderedView.data.User);
            options.timezoneSelectList = app.prerenderedView.data.TimezoneSelectList;
            options.licenceSelectList = app.prerenderedView.data.LicenceSelectList;
        }

        var userFormView = new UserFormView(options);

        app.showContentView('Edit Account Details', userFormView, 'account');
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