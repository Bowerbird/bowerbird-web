/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// SightingNote
// ------------

define(['jquery', 'underscore', 'backbone', 'models/identification'],
function ($, _, Backbone, Identification) {

    var SightingNote = Backbone.Model.extend({
        defaults: {
            Id: null,
            IsCustomIdentification: false,
            SightingId: '',
            Descriptions: [],
            Tags: '',
            Taxonomy: ''
        },

        //urlRoot: '/sightingnotes' ,

        url: function () {
            var url = '/' + this.get('SightingId');
            if (this.id) {
                url += '/updatenote/' + this.id;
            } else {
                url += '/createnote';
            }
            return url;
        },

        idAttribute: 'Id',

        initialize: function (attributes) {
            if (attributes && attributes.Identification) {
                this.identification = new Identification(attributes.Identification);
            }
            if (attributes && attributes.Descriptions) {

            }
        },

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
        },

        setIdentification: function (identification) {
            this.identification = identification;
            var taxonomy = '';
            if (identification) {
                taxonomy = identification.get('Taxonomy');
            }
            this.set('Taxonomy', taxonomy);
        }
    });

    return SightingNote;

});