/// <reference path="../libs/log.js" />
/// <reference path="../libs/jquery-1.7.1.min.js" />
/// <reference path="../libs/underscore.js" />
/// <reference path="../libs/backbone.js" />
/// <reference path="../libs/backbone.marionette.js" />
/// <reference path="../libs/icanhaz.js" />
/// <reference path="namespace.js" />
/// <reference path="app.js" />

// Backbone.Marionette.Renderer override
// -------------------------------------

(function (Backbone, ich) {

    // Override the renderer so that it uses mustache templates 
    // together with icanhaz caching
    Backbone.Marionette.Renderer.render = function (template, data) {
        if (template) { // Marionette seems to call this method even if a view is created with a pre-existing DOM element. May need to investigate further.
            return ich[template](data);
        }
    };

})(Backbone, ich);
