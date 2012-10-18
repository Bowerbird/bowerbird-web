/// <reference path="../libs/log.js" />
/// <reference path="../libs/require/require.js" />
/// <reference path="../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../libs/underscore/underscore.js" />
/// <reference path="../libs/backbone/backbone.js" />
/// <reference path="../libs/backbone.marionette/backbone.marionette.js" />

// Require AMD config
// ------------------

// Setup config
require.config({
    baseUrl: '/js/bowerbird',
    paths: {
        jquery: 'http://ajax.googleapis.com/ajax/libs/jquery/1.7.2/jquery', // jQuery is now AMD compliant
        json2: '../libs/json/json2',
        underscore: '../libs/underscore/underscore', // AMD version from https://github.com/amdjs
        backbone: '../libs/backbone/backbone', // AMD version from https://github.com/amdjs,
        queryparams: '../libs/backbone.queryparams/backbone.queryparams',
        marionette: '../libs/backbone.marionette/backbone.marionette',
        noext: '../libs/require/noext', //https://github.com/millermedeiros/requirejs-plugins
        async: '../libs/require/async', // Required by google loader
        goog: '../libs/require/goog', // Google async loader
        propertyParser: '../libs/require/propertyparser', // Required by google loader
        ich: '../libs/icanhaz/icanhaz', // Mustache templates cache and renderer
        jqueryui: '../libs/jqueryui',
        datepicker: '../libs/bootstrap/bootstrap-datepicker',
        date: '../libs/date/date',
        multiselect: '../libs/jquery.multiselect/jquery.multiselect',
        loadimage: '../libs/jquery.fileupload/load-image',
        fileupload: '../libs/jquery.fileupload/jquery.fileupload',
        iframetransport: '../libs/jquery.fileupload/jquery.iframe-transport',
        signalr: '../libs/jquery.signalr/jquery.signalr',
        timeago: '../libs/jquery.timeago/jquery.timeago',
        jsonp: '../libs/jquery.jsonp/jquery.jsonp',
        log: '../libs/log/log',
        hubs: 'hubs',
        grab: '../libs/jquery.jplayer/jquery.grab',
        transform: '../libs/jquery.jplayer/jquery.transform',
        jplayer: '../libs/jquery.jplayer/jquery.jplayer',
        circleplayer: '../libs/jquery.jplayer/circle.player',
        progress: '../libs/jquery.animateprogress/jquery.animate-progress',
        carousel: '../libs/jquery.carousel/jquery.carousel',
        touchswipe: '../libs/jquery.carousel/jquery.touchswipe.min',
        moment: '../libs/moment/moment'
    }
});

// Start app with primary dependancies
require([
        'app',
        'bootstrap-data', // Get bootstrapped data from inline AMD module
        'ich',
        'log',
        'jquery',
        'json2', 
        'underscore',
        'backbone',
        'queryparams',
        'marionette',
        'noext!/templates', // Load templates from server
        'controllers/usercontroller',
        'controllers/activitycontroller',
        'controllers/debugcontroller',
        'controllers/groupusercontroller',
        'controllers/homecontroller',
        'controllers/observationcontroller',
        'controllers/recordcontroller',
        'controllers/organisationcontroller',
        'controllers/postcontroller',
        'controllers/projectcontroller',
        'controllers/referencespeciescontroller',
        'controllers/speciescontroller',
        'controllers/teamcontroller',
        'controllers/accountcontroller',
        'controllers/chatcontroller',
        'views/headerview',
        'views/footerview',
        'views/sidebarview',
        'views/notificationscompositeview',
        'views/onlineusersview',
        'signalr',
        'hubs'
    ],
    function (app, bootstrapData) {
        log('bootstrapped data', bootstrapData);
        // Start the app as soon as the DOM is ready, loading in the bootstrapped data
        $(function () {
            app.start(bootstrapData);
        });
    });