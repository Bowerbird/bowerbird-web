/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// SightingNote
// ------------

define(['jquery', 'underscore', 'backbone'],
function ($, _, Backbone) {

    var SightingNote = Backbone.Model.extend({
        defaults: {
            IsCustomIdentification: false,
            SightingId: '',
            Descriptions: [],
            Tags: ''
        },

        urlRoot: '/sightingnotes',

        idAttribute: 'Id',

        addDescription: function (id) {
            var list = this.get('Descriptions');
            var desc = { key: id, value: '' };
            list.push(desc);
            this.set('Descriptions', list);
        },

        setDescription: function (id, text) {
            var list = this.get('Descriptions');
            var newList = _.map(list, function (item) {
                if (item.key === id) {
                    item.value = text;
                }
                return item;
            });
            this.set('Descriptions', newList);
        },

        setIdentification: function (identification) {
            this.set('Taxonomy', identification.get('Taxonomy'));
            this.identification = identification;
        }
    });

    return SightingNote;

});