/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// Record
// ------

define(['jquery', 'underscore', 'backbone'],
function ($, _, Backbone) {
    
    var Record = Backbone.Model.extend({
        defaults: {
            ObservedOn: null,
            Address: '',
            Latitude: null,
            Longitude: null,
            Category: '',
            AnonymiseLocation: false,
            ProjectIds: [],
            Comments: []
        },

        urlRoot: '/records',

        idAttribute: 'Id',

        toJSON: function () {
            return {
                ObservedOn: this.get('ObservedOn'),
                Latitude: this.get('Latitude'),
                Longitude: this.get('Longitude'),
                Category: this.get('Category'),
                AnonymiseLocation: this.get('AnonymiseLocation'),
                ProjectIds: this.get('ProjectIds'),
                Comments: this.get('Comments')
            };
        },

        addProject: function (id) {
            var projects = this.get('ProjectIds');
            projects.push(id);
            this.set('ProjectIds', projects);
        },

        removeProject: function (id) {
            var projects = this.get('ProjectIds');
            this.set('ProjectIds', _.without(projects, id));
        }
    });

    return Record;

});