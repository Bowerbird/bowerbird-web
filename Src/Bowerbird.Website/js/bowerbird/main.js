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
        jquery : 'http://ajax.googleapis.com/ajax/libs/jquery/1.7.2/jquery', // jQuery is now AMD compliant
        underscore: '/js/libs/underscore/underscore', // AMD version from https://github.com/amdjs
        backbone: '/js/libs/backbone/backbone', // AMD version from https://github.com/amdjs,
        marionette: '/js/libs/backbone.marionette/backbone.marionette',
        text: '/js/libs/require/text', // Require.js text loader plugin
        ich: '/js/libs/icanhaz/icanhaz', // Mustache templates cache and renderer
    },
    priority: [
        'ich', 
        'jquery', 
        'underscore',
        'backbone', 
        'marionette', 
        // Load the bootstraped data from the inline html
        'bootstrap-data', 
        // Routers are the first port of call, so load em up
        'routers/groupuserrouter',
        'routers/contributionrouter',
        // Load top level views, beacuse no one else is gonna do it
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