/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// MediaResourceCollection
// -----------------------

define(['jquery', 'underscore', 'backbone', 'models/mediaresource'], function ($, _, Backbone, MediaResource) {

    var MediaResourceCollection = Backbone.Collection.extend({
        model: MediaResource,

        url: '/mediaresources',

        initialize: function () {
            _.extend(this, Backbone.Events);
        }
    });

    return MediaResourceCollection;

});