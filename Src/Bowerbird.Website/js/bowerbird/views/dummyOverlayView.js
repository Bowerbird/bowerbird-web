/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// DummyOverlayView
// ----------------

// Google maps dummy overlay to allow drag and drop of pin
define(['jquery', 'underscore', 'backbone', 'app', 'async!http://maps.google.com/maps/api/js?sensor=false&region=AU'], function ($, _, Backbone, app) {

    var DummyOverlayView = function (map) {
        // Bind this to the map to access MapCanvasProjection
        this.setMap(map);
        // MapCanvasProjection is only available after draw has been called.
        this.draw = function () { };
    };

    DummyOverlayView.prototype = new google.maps.OverlayView();

    return DummyOverlayView;

});