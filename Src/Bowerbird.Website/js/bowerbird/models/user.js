/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// User
// ----

define(['jquery', 'underscore', 'backbone', 'app', 'collections/projectcollection'], function ($, _, Backbone, app, ProjectCollection) {

    var User = Backbone.Model.extend({
        idAttribute: 'Id',

        initialize: function (model) {
            this.projects = new ProjectCollection();
            if (model) {
                this.projects.add(model.Projects);
            }
        }
    });

    return User;

});