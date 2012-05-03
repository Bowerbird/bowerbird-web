/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ProjectLayout
// -------------

// The left hand side bar that is shown to authenticated users.
define(['jquery', 'underscore', 'backbone', 'app', 'models/project', 'views/streamview'], function ($, _, Backbone, app, Project, StreamView) {

//    var DetailsRegionManager = Backbone.Marionette.Region.extend({
//        el: '.details'
//    });

    var ProjectLayout = Backbone.Marionette.Layout.extend({
        tagName: 'section',

        id: 'content',

        className: 'triple-2 project',

        template: 'Project',

        regions: {
            summary: '.summary',
            details: '.details'
        },

        //        initialize: function (options) {
        //        },

        //        onRender: function () {
        //        },

        showStream: function (streamItems) {
            var streamView = new StreamView({ model: this.model, collection: streamItems });
            this.details.show(streamView);
            streamView.render();
        }
    });

    return ProjectLayout;

});