/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// AccountController & AccountRouter
// ---------------------------------

define(['jquery', 'underscore', 'backbone', 'app', 'models/user', 'models/accountupdate', 'models/accountupdatepassword', 'models/accountrequestpasswordupdate', 'models/login',
        'models/register', 'views/accountloginview', 'views/accountregisterview',
        'views/accountupdateformview', 'views/accountupdatepasswordformview', 'views/accountrequestpasswordupdateformview'],
function ($, _, Backbone, app, User, AccountUpdate, AccountUpdatePassword, AccountRequestPasswordUpdate, Login, Register, AccountLoginView, AccountRegisterView,
    AccountUpdateFormView, AccountUpdatePasswordFormView, AccountRequestPasswordUpdateFormView) {

    var AccountRouter = Backbone.Marionette.AppRouter.extend({
        appRoutes: {
            'account/login': 'showLogin',
            'account/register': 'showRegister',
            'account/update': 'showUserUpdateForm',
            'account/updatepassword/:key': 'showPasswordUpdateForm',
            'account/updatepassword': 'showPasswordUpdateForm',
            'account/requestpasswordupdate': 'showRequestPasswordUpdateForm'
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

    var updateVote = function (uri, score) {
        var deferred = new $.Deferred();
        $.ajax({
            url: uri,
            type: 'POST',
            data: { score: score }
        }).done(function (data) {
            deferred.resolve(data.Model);
        });
        return deferred.promise();
    };

    var updateFavourites = function (sighting) {
        var deferred = new $.Deferred();
        $.ajax({
            url: '/favourites',
            type: 'POST',
            data: { id: sighting.id }
        }).done(function (data) {
            deferred.resolve(data.Model);
        });
        return deferred.promise();
    };

    var followUser = function (user) {
        var deferred = new $.Deferred();
        $.ajax({
            url: '/follow',
            type: 'POST',
            data: { id: user.id }
        }).done(function (data) {
            deferred.resolve(data.Model);
        });
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

        var email = '';
        var returnUrl = '';

        if (app.isPrerendering()) {
            email = app.prerenderedView.data.AccountLogin.Email;
            returnUrl = app.prerenderedView.data.AccountLogin.ReturnUrl;
        } else {
            if (docCookies.hasItem('56277e138f774318ab152a84dad7adf9')) {
                email = docCookies.getItem('56277e138f774318ab152a84dad7adf9');
            }
        }

        var login = new Login({
            Email: email,
            ReturnUrl: returnUrl
        });

        var options = {
            model: login
        };

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

        var options = {
            model: new Register()
        };

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
            options.model = new AccountUpdate(app.prerenderedView.data.User);
            options.timezoneSelectList = app.prerenderedView.data.TimezoneSelectList;
            options.licenceSelectList = app.prerenderedView.data.LicenceSelectList;

            var accountUpdateFormView = new AccountUpdateFormView(options);
            app.showContentView('Edit Account Details', accountUpdateFormView, 'account');
        } else {
            var accountUpdate = new AccountUpdate({ Id: app.authenticatedUser.user.id });
            accountUpdate.fetch({
                success: function (model, data) {
                    options.model = model;
                    options.timezoneSelectList = data.Model.TimezoneSelectList;
                    options.licenceSelectList = data.Model.LicenceSelectList;

                    var accountUpdateFormView = new AccountUpdateFormView(options);
                    app.showContentView('Edit Account Details', accountUpdateFormView, 'account');
                }
            });
        }

    };

    AccountController.showPasswordUpdateForm = function (key) {
        log('....................................');
        var options = {};
        if (app.isPrerenderingView('account')) {
            options['el'] = '.update-password';
            options.model = new AccountUpdatePassword(app.prerenderedView.data.AccountUpdatePassword);
            options.errors = app.prerenderedView.data.Errors;

            var accountUpdatePasswordFormView = new AccountUpdatePasswordFormView(options);
            app.showContentView('Edit Password', accountUpdatePasswordFormView, 'account');
        } else {
            var accountUpdatePassword = new AccountUpdatePassword();
            accountUpdatePassword.fetch({
                success: function (model, data) {
                    options.model = model;

                    var accountUpdatePasswordFormView = new AccountUpdatePasswordFormView(options);
                    app.showContentView('Edit Password', accountUpdatePasswordFormView, 'account');
                }
            });
        }

    };

    AccountController.showRequestPasswordUpdateForm = function () {
        var options = {};
        if (app.isPrerenderingView('account')) {
            options['el'] = '.request-password-update';
            options.model = new AccountRequestPasswordUpdate(app.prerenderedView.data.AccountUpdatePassword);

            var accountRequestPasswordUpdateFormView = new AccountRequestPasswordUpdateFormView(options);
            app.showContentView('Request Password Reset', accountRequestPasswordUpdateFormView, 'account');
        } else {
            var accountRequestPasswordUpdate = new AccountRequestPasswordUpdate();
            accountRequestPasswordUpdate.fetch({
                success: function (model, data) {
                    options.model = model;

                    var accountRequestPasswordUpdateFormView = new AccountRequestPasswordUpdateFormView(options);
                    app.showContentView('Request Password Reset', accountRequestPasswordUpdateFormView, 'account');
                }
            });
        }

    };

    // Event Handlers
    // --------------

    app.vent.on('close-call-to-action', function (name) {
        $.when(getModel('/account/closecalltoaction', 'POST', { name: name }));
    });

    app.vent.on('update-sighting-vote', function (sighting, score) {
        $.when(updateVote('/' + sighting.id + '/vote', score));
    });

    app.vent.on('update-sighting-note-vote', function (sightingNote, score) {
        $.when(updateVote('/' + sightingNote.get('SightingId') + '/' + sightingNote.id + '/vote', score));
    });

    app.vent.on('update-identification-vote', function (identification, score) {
        $.when(updateVote('/' + identification.get('SightingId') + '/' + identification.id + '/vote', score));
    });

    app.vent.on('update-favourites', function (sighting) {
        $.when(updateFavourites(sighting));
    });

    app.vent.on('follow-user', function (user) {
        $.when(followUser(user));
    });

    app.vent.on('unfollow-user', function (user) {
        $.when(followUser(user));

        var userProject = app.authenticatedUser.userProjects.find(function (item) { return item.get('CreatedBy') === user.id; });
        log(userProject);
        app.authenticatedUser.userProjects.remove(userProject);
    });

    app.addInitializer(function () {
        this.accountRouter = new AccountRouter({
            controller: AccountController
        });
    });

    return AccountController;

});