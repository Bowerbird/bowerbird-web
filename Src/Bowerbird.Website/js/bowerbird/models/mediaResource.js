/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// MediaResource
// -------------

define(['jquery', 'underscore', 'backbone'], function ($, _, Backbone) {

    var MediaResource = Backbone.Model.extend({
        defaults: {
            Key: '',
            MediaType: ''
        },

        idAttribute: 'Id',

        urlRoot: '/mediaresources'
    });

    return MediaResource;

});