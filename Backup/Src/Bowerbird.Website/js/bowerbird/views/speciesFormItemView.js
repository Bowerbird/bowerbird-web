/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// SpeciesFormItemView
// -------------------------

define(['jquery', 'underscore', 'backbone', 'app', 'ich', 'multiselect'], function ($, _, Backbone, app, ich) {

    var SpeciesFormItemView = Backbone.Marionette.ItemView.extend({

        tagName: 'section',

        className: 'form single-medium',

        id: 'species-form',

        template: 'SpeciesCreate',

        events: {
            'click #cancel': '_cancel',
            'click #save': '_save',
            'change input#kingdom': '_contentChanged',
            'change input#group': '_contentChanged',
            'change input#commonnames': '_contentChanged',
            'change input#taxonomy': '_contentChanged',
            'change input#order': '_contentChanged',
            'change input#family': '_contentChanged',
            'change input#genusname': '_contentChanged',
            'change input#speciesname': '_contentChanged',
            'change #proposed-new-species-field input:checkbox': '_proposedNewSpeciesChanged'
        },

        onRender: function () {
        },
        
        _contentChanged: function (e) {
            var target = $(e.currentTarget);
            var data = {};
            data[target.attr('id')] = target.attr('value');
            this.model.set(data);
        },

        _proposedNewSpeciesChanged: function (e) {
        },

        _cancel: function () {
        },

        _save: function () {
            this.model.save();
        }
    });

    return SpeciesFormItemView;
});