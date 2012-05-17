/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// ReferenceSpeciesFormLayoutView
// -------------------------

define(['jquery', 'underscore', 'backbone', 'app', 'ich', 'multiselect'], function ($, _, Backbone, app, ich) {

    var ReferenceSpeciesFormLayoutView = Backbone.Marionette.Layout.extend({

        tagName: 'section',

        className: 'form single-medium',

        id: 'reference-species-form',

        regions: {
            avatar: '#avatar-fieldset'
        },

        events: {
            'click #cancel': '_cancel',
            'click #save': '_save',
            'change input#species': '_contentChanged',
            'change input#smarttags': '_contentChanged',
        },

        onRender: function () {
            alert('referenceSpeciesFormLayoutView ' + Date());

            //            if (app.authenticatedUser.teams != 'undefined') {
            //                // Define Team Option List template and add user's teams
            //                //ich.addTemplate('TeamSelectItem', '{{#Teams}}<option value="{{Id}}">{{Name}}</option>{{/Teams}}');
            //                //log(app);
            //                //this.$el.find('#Teams').append(ich.TeamSelectItem({ Teams: app.prerenderedView.data.Teams.Items.PagedListItems }));

            //                // Apply funky select list with avatar to Team select view
            //                this.teamListSelectView = $("#Team").multiSelect({
            //                    selectAll: false,
            //                    singleSelect: true,
            //                    noneSelected: '<span>Select a Team</span>',
            //                    oneOrMoreSelected: function (selectedOptions) {
            //                        var $selectedHtml = $('<span />');
            //                        _.each(selectedOptions, function (option) {
            //                            $selectedHtml.append('<span>' + option.text + '</span> ');
            //                        });
            //                        return $selectedHtml.children();
            //                    }
            //                });
            //            }
        },

        _contentChanged: function (e) {
            var target = $(e.currentTarget);
            var data = {};
            data[target.attr('id')] = target.attr('value');
            this.model.set(data);
        },

        _cancel: function () {
        },

        _save: function () {
            this.model.save();
        }
    });

    return ReferenceSpeciesFormLayoutView;
});