﻿/// <reference path="../libs/log.js" />
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
        jquery : 'http://ajax.googleapis.com/ajax/libs/jquery/1.7.2/jquery', // jQuery is now AMD compliant
        underscore: '/js/libs/underscore/underscore', // AMD version from https://github.com/amdjs
        backbone: '/js/libs/backbone/backbone', // AMD version from https://github.com/amdjs,
        marionette: '/js/libs/backbone.marionette/backbone.marionette',
        text: '/js/libs/require/text', // Require.js text loader plugin
        ich: '/js/libs/icanhaz/icanhaz', // Mustache templates cache and renderer
        goog: '/js/libs/require/goog', // Google async loader
        propertyParser : '/js/libs/require/propertyparser', // Required by google loader
        async: '/js/libs/require/async', // Required by google loader
        jqueryui: '/js/libs/jqueryui',
        datepicker: '/js/libs/bootstrap/bootstrap-datepicker',
        date: '/js/libs/date/date',
        multiselect: '/js/libs/jquery.multiselect/jquery.multiselect',
        loadimage: '/js/libs/jquery.fileupload/load-image', 
        fileupload: '/js/libs/jquery.fileupload/jquery.fileupload'
    },
    priority: [
        'ich', 
        'jquery', 
        'underscore',
        'backbone', 
        'marionette', 
        // Load the bootstrapped data from the inline html
        //'bootstrap-data', 
        // Routers are the first port of call, so load em up
        'routers/groupuserrouter',
        'routers/contributionrouter',
        'routers/projectrouter',
        'routers/teamrouter',
        'routers/organisationrouter',
        'routers/speciesrouter',
        'routers/referencespeciesrouter',
        // Load top level views, beacuse no one else is gonna do it
        'views/headerview',
        'views/footerview',
        'views/sidebarlayoutview',
        'views/projectlayoutview',
        'views/observationlayoutview'
    ]
});

// Init dependencies
require(['backbone', 'ich', 'marionette', '/templates'], function (Backbone, ich) {

    // Override the marionette renderer so that it uses mustache templates 
    // together with icanhaz caching
    Backbone.Marionette.Renderer.render = function (template, data) {
        if (template) { // Marionette seems to call this method even if a view is created with a pre-existing DOM element. May need to investigate further.
            return ich[template](data);
        }
    };

});