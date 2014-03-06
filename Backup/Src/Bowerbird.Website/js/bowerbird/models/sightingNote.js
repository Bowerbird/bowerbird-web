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
            Id: null,
            SightingId: '',
            Descriptions: [],
            Tags: '',
            NoteComments: ''
        },

        url: function () {
            var url = '/' + this.get('SightingId') + '/notes';
            if (this.id) {
                url += '/' + this.id;
            }
            return url;
        },

        idAttribute: 'Id',

        addDescription: function (id) {
            var list = this.get('Descriptions');
            var desc = { Key: id, Value: '' };
            list.push(desc);
            this.set('Descriptions', list);
        },

        setDescription: function (id, text) {
            var list = this.get('Descriptions');
            var newList = _.map(list, function (item) {
                if (item.Key === id) {
                    item.Value = text;
                }
                return item;
            });
            this.set('Descriptions', newList);
        }
    });

    return SightingNote;

});