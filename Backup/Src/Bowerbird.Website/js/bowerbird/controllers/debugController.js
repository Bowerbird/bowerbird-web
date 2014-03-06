/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// DebugController & DebugRouter
// -----------------------------

define(['jquery', 'underscore', 'backbone', 'app'],
function ($, _, Backbone, app) {

    var DebugRouter = function (options) {

        this.hub = $.connection.debugHub;
        this.controller = options.controller;
        this.hub.client.debugToClient = this.controller.debugToClient;
    };

    var DebugController = {};

    DebugController.hub = $.connection.debugHub;

    // DebugController Public API To HUB
    //----------------------------------

    DebugController.debugToLog = function (message) {
        log(message);
    };

    // ChatController Public API From HUB
    // ----------------------------------

    DebugController.debugToClient = function(message) {
        log(message);
    };

    app.addInitializer(function () {
        this.debugRouter = new DebugRouter({
            controller: DebugController
        });
    });

    return DebugController;

});