/// <reference path="../libs/log.js" />
/// <reference path="../libs/require/require.js" />
/// <reference path="../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../libs/underscore/underscore.js" />
/// <reference path="../libs/backbone/backbone.js" />
/// <reference path="../libs/backbone.marionette/backbone.marionette.js" />
/// <reference path="../libs/jquery.signalr/jquery-1.6.2-vsdoc.js" />

// Bowerbird.app
// -------------

// Initialises the app, but does not start rendering. That is done 
// when app.start() is called
define(['jquery', 'underscore', 'backbone', 'ich', 'bootstrap-data', 'models/user', 'collections/usercollection', 'collections/projectcollection', 'collections/organisationcollection', 'collections/userprojectcollection', 'collections/activitycollection', 'collections/chatcollection', 'marionette', 'signalr'],
function ($, _, Backbone, ich, bootstrapData, User, UserCollection, ProjectCollection, OrganisationCollection, UserProjectCollection, ActivityCollection, ChatCollection) {
    // Create an instance of the app
    var app = new Backbone.Marionette.Application();

    // Let's pollute the global namespace, just a little, for debug purposes :)
    window.Bowerbird = window.Bowerbird || { app: app };

    app.addRegions({
        header: '#header',
        footer: 'footer',
        sidebar: '#sidebar',
        content: '#content',
        notifications: '#notifications',
        usersonline: '#onlineusers'
    });

    // Sets the prerender complete flag to "done"
    var setPrerenderComplete = function () {
        app.prerenderedView.isBound = true;
    };

    // Determines which Backbone method to call to init the view (bootstrap it, or render it)
    var getShowViewMethodName = function (name) {
        if (!name) {
            var err = new Error("A name must be provided!");
            err.name = "BowerbirdNoViewNameProvidedError";
            throw err;
        }
        return app.isPrerenderingView(name) ? 'attachView' : 'show';
    };

    //Updates the HTML title tag
    var updateTitle = function (titleSegment) {
        var newTitle = 'BowerBird';
        if (titleSegment.length > 0) {
            newTitle = titleSegment + ' - ' + newTitle;
        }
        document.title = newTitle;
    };

    // Renders a view, taking into account if it should be bootstrapped in, or rendered
    var renderView = function (title, view, name, onShowCallback, showSidebar) {
        // Update title
        updateTitle(title);

        if (!app.isPrerenderingView(name)) {
            view.$el.hide();
        }

        // Show/attach view
        app.content[getShowViewMethodName(name)](view);

        // If page is loaded from server, bootstrap UI
        if (app.isPrerenderingView(name)) {
            view.showBootstrappedDetails();
        }

        // Perform anim
        if (showSidebar) {
            $('#sidebar').fadeIn(100);
        }

        // Show content
        view.$el.fadeIn(100, function () {
            onShowCallback();
            setPrerenderComplete();
            app.vent.trigger('view:render:complete', view);
        });
    };

    // A history of the routes that have been navigated
    var routeHistory = [];

    // Holds a ref to the previous content view
    var previousContentView = null;

    // Resize the sidebar
    var resizeSidebar = function () {
        if ($('#sidebar').length > 0) {
            log('resizing sidebar', $('header.default-header h1').offset(), $('header.default-header h1').width());

            $('#sidebar').css('width', $('header.default-header h1').width() + 'px');
            $('#sidebar').css('left', $('header.default-header h1').offset().left + 'px');
        }
    };

    app.isPrerenderingView = function (name) {
        return name === app.prerenderedView.name && !app.prerenderedView.isBound;
    };

    app.isPrerendering = function () {
        return !app.prerenderedView.isBound;
    };

    app.showContentView = function (title, view, name, onShow) {
        var onShowCallback = onShow || function () { };

        var isForm = view.viewType === 'form';

        if (app.content.currentView) {
            // Don't show sidebar on forms because they are full width
            if (isForm) {
                $('#sidebar').fadeOut(100);
            }
            app.content.currentView.$el.fadeOut(100, function () {
                // If navigating to a form, we hide and cache the current view so that we can re-show it once user has completed
                // form task. Provides for better UX.
                if (isForm) {
                    // Store previous view in cache
                    previousContentView = {
                        viewType: app.content.currentView.viewType || 'unknown',
                        title: document.title.replace('BowerBird', '').replace('-', ''),
                        view: app.content.currentView,
                        $el: app.content.currentView.$el.detach()
                    };

                    // Clear out current view
                    app.content.currentView = null;
                }

                renderView(title, view, name, onShowCallback, !isForm);
            });
        } else { // Load page from server, this is the first view
            renderView(title, view, name, onShowCallback, !isForm);
        }
    };

    app.showPreviousContentView = function () {
        if (previousContentView) {
            // Close current view
            app.content.close();

            // Reinstate previous view
            app.content.$el.append(previousContentView.$el);
            app.content.currentView = previousContentView.view;
            routeHistory.shift();
            Backbone.history.navigate(_.first(routeHistory));

            updateTitle(previousContentView.title);

            // Clear out previous view cache
            previousContentView = null;

            // Perform anim
            $('#sidebar').fadeIn(100);
            app.content.currentView.$el.fadeIn(100, function () {
                app.content.currentView.trigger('reshow');
            });
        } else {
            // If we don't have a previous view, take user back to home stream
            Backbone.history.navigate('', { trigger: true });
        }
    };

    app.generateGuid = function () {
        var S4 = function () {
            return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
        };
        return (S4() + S4() + "-" + S4() + "-" + S4() + "-" + S4() + "-" + S4() + S4() + S4());
    };

    // Load the bootstrapped data into place
    app.bind('initialize:before', function () {
        // Override the marionette renderer so that it uses mustache templates 
        // together with icanhaz caching
        Backbone.Marionette.Renderer.render = function (template, data) {
            if (template) { // Marionette seems to call this method even if a view is created with a pre-existing DOM element. May need to investigate further.
                return ich[template](data);
            }
        };

        // Add additional capability if authenticated user
        if (bootstrapData.Model.AuthenticatedUser) {
            // Online users
            app.onlineUsers = new UserCollection();

            // Chats
            app.chats = new ChatCollection();
            app.chatRegions = [];
        }

        // Add the prerendered view string to the app for use by controller duing init of first view
        app.prerenderedView = {
            name: bootstrapData.Model.PrerenderedView,
            isBound: false, // Flag used to determine if prerenderd view has been bound to the object/DOM model
            data: bootstrapData.Model
        };
    });

    app.bind('initialize:after', function () {
        // Tasks to perform on DOM ready
        var that = this;

        $(function () {
            // Only start history once app is fully initialised
            if (Backbone.history) {
                Backbone.history.on('route', function (route, name) {
                    routeHistory.unshift(Backbone.history.fragment);
                });

                // Start URL and history routing
                Backbone.history.start({ pushState: true, hashChange: true });
            }

            // initialise the hub connection
            $.connection.hub.start({ transport: ['webSockets', 'longPolling'] }, function () {

                // Keep the client id
                app.clientId = $.signalR.hub.id;
                log('browser connected via signalr as ' + app.clientId);

                // Subscribe authenticated user to all their groups
                if (app.authenticatedUser) {
                    $.connection.userHub.registerUserClient(app.authenticatedUser.user.id)
                        .done(function () {
                            log('Added user to hub');
                            app.onlineUsers.add(app.authenticatedUser.user);
                        })
                        .fail(function (e) {
                            log('could not register client with hub', e);
                        });
                }
            });

            $.connection.hub.error(function () {
                log('ERROR: SignalR Hub blew up!');

                reconnectToHub();
            });

            // Register closing of all popup menus in entire page
            $('html').click(function () {
                app.vent.trigger('close-sub-menus');
            });

            // Catch close menu event and close all types of menus here
            app.vent.on('close-sub-menus', function () {
                $('html .sub-menu').removeClass('active'); // Make sure to add any new menu button types to the selector
                $('html .multiSelect').multiSelectOptionsHide();
            });

            // Resize the sidebar on window resizing
            var resizeSidebarTimer;
            $(window).resize(function () {
                clearTimeout(resizeSidebarTimer);
                resizeSidebarTimer = setTimeout(function () {
                    resizeSidebar();
                }, 100);
            });

            resizeSidebar();
        });
    });

    var reconnectToHub = function () {
        $.connection.hub.stop();
        log('Doing a Hub Re-Connect');
        $.connection.hub.start({ transport: ['webSockets', 'longPolling'] }, function () {
            // Keep the client id
            app.clientId = $.signalR.hub.id;
            log('browser re-connected via signalr as ' + app.clientId);

            // Subscribe authenticated user to all their groups
            if (app.authenticatedUser) {
                $.connection.userHub.registerUserClient(app.authenticatedUser.user.id)
                        .done(function () {
                            log('Added user to hub');
                            app.onlineUsers.add(app.authenticatedUser.user);
                        })
                        .fail(function (e) {
                            log('could not register client with hub', e);
                        });
            }
        });
    };

    return app;

});