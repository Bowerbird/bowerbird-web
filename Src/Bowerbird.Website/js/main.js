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
    //urlArgs: "bust=" + (new Date()).getTime(), // Cache buster
    paths: {
        jquery: 'http://ajax.googleapis.com/ajax/libs/jquery/1.7.2/jquery', // jQuery is now AMD compliant
        json2: '../libs/json/json2',
        underscore: '../libs/underscore/underscore', // AMD version from https://github.com/amdjs
        backbone: '../libs/backbone/backbone', // AMD version from https://github.com/amdjs,
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
        signalr: '../libs/jquery.signalr/jquery.signalr',
        timeago: '../libs/jquery.timeago/jquery.timeago',
        log: '../libs/log/log',
        hubs: 'hubs'
    },
    shim: {
        //'/signalr/hubs?noext': ['signalr', 'jquery', 'json2'] // Load non-AMD signalr hubs script
        hubs: ['signalr', 'jquery'] // Load non-AMD signalr hubs script
    }
});

// Start app with primary dependancies
require([
        'app',
        'bootstrap-data', // Get bootstrapped data from inline AMD module
        'log',
        'backbone',
        'jquery',
        'ich',
        'marionette',
        '/templates?noext', // Load templates from server
        'controllers/usercontroller',
        'controllers/activitycontroller',
        'controllers/debugcontroller',
        'controllers/groupusercontroller',
        'controllers/homecontroller',
        'controllers/observationcontroller',
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
        'views/sidebarlayoutview',
        'views/notificationscompositeview',
        'views/onlineusercompositeview',
        'views/chatcompositeview',
        //'/signalr/hubs?noext'
        'hubs'
    ],
    function (app, bootstrapData) {
        log(bootstrapData);
        // Start the app as soon as the DOM is ready, loading in the bootstrapped data
        $(function () {
            app.start(bootstrapData);
        });
    });