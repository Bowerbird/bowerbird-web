/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// UserProject
// -----------

define(['jquery', 'underscore', 'backbone'], function ($, _, Backbone) {

    var UserProject = Backbone.Model.extend({
        defaults: {
            Id: null,
            Name: '',
            Type: 'UserProject'
        },

        idAttribute: 'Id',

        urlRoot: '/userprojects'
    });

    return UserProject;

});