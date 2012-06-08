/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ActivityCollection
// ------------------

define(['jquery', 'underscore', 'backbone', 'app', 'models/activity'], function ($, _, Backbone, app, Activity) {

    var ActivityCollection = Backbone.Collection.extend({
        model: Activity,

        initialize: function () {
            _.extend(this, Backbone.Events);
        }
    });

    return ActivityCollection;

});