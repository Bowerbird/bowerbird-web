/// <reference path="../libs/log.js" />
/// <reference path="../libs/require/require.js" />
/// <reference path="../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../libs/underscore/underscore.js" />
/// <reference path="../libs/backbone/backbone.js" />
/// <reference path="../libs/backbone.marionette/backbone.marionette.js" />

// Require Configuration
// ---------------------

// Setup
require.config({
    baseUrl: '/js/bowerbird',
    //urlArgs: "bust=" + (new Date()).getTime(), // Cache buster
    paths: {
        jquery: 'http://ajax.googleapis.com/ajax/libs/jquery/1.7.2/jquery', // jQuery is now AMD compliant
        json2: '../libs/json/json2',
        underscore: '../libs/underscore/underscore', // AMD version from https://github.com/amdjs
        backbone: '../libs/backbone/backbone', // AMD version from https://github.com/amdjs,
        marionette: '../libs/backbone.marionette/backbone.marionette',
        text: '../libs/require/text', // Require.js text loader plugin
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
        timeago: '../libs/jquery.timeago/jquery.timeago'
    }
    // COMMENT THIS OUT FOR VERBOSE DEBUG VERSION
    ,
    priority: [
        'ich',
        'jquery', 
        'json2',
        'underscore',
        'backbone',
        'marionette',
        'signalr',
        // Routers are the first port of call, so load em up
        'controllers/activitycontroller',
        'controllers/groupusercontroller',
        'controllers/homecontroller',
        'controllers/observationcontroller',
        'controllers/organisationcontroller',
        'controllers/postcontroller',
        'controllers/projectcontroller',
        'controllers/referencespeciescontroller',
        'controllers/speciescontroller',
        'controllers/teamcontroller',
        // Load top level views, beacuse no one else is gonna do it
        'views/headerview',
        'views/footerview',
        'views/sidebarlayoutview',
        'views/notificationscompositeview',
        'views/homelayoutview',
        'views/projectlayoutview',
        'views/observationlayoutview',
        'views/onlineuserscompositeview',
        'views/exploreprojectview'
    ]
});

// Init dependencies
require(['backbone', 'ich', 'marionette', 'noext!/templates', 'noext!/signalr/hubs'], function (Backbone, ich) {

    // Override the marionette renderer so that it uses mustache templates 
    // together with icanhaz caching
    Backbone.Marionette.Renderer.render = function (template, data) {
        if (template) { // Marionette seems to call this method even if a view is created with a pre-existing DOM element. May need to investigate further.
            return ich[template](data);
        }
    };

});