/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// SidebarOrganisationItemView
// ---------------------------

define(['jquery', 'underscore', 'backbone', 'app', 'models/organisation'], function ($, _, Backbone, app, Organisation) {

    var SidebarOrganisationItemView = Backbone.Marionette.ItemView.extend({
        tagName: 'li',

        className: 'menu-group-item',

        template: 'SidebarOrganisationItem',

        events: {
            'click .chat-menu-item': 'startChat',
            'click .sub-menu-button': 'showMenu',
            'click .sub-menu-button li': 'selectMenuItem'
        },

        onRender: function () {
            var that = this;
            $(this.el).children('a').on('click', function (e) {
                e.preventDefault();
                app.groupUserRouter.navigate($(this).attr('href'));
                app.vent.trigger('organisation:show:stream', that.model.id);
                return false;
            });
        },

        serializeData: function () {
            return {
                Id: this.model.id,
                Name: this.model.get('Name'),
                Description: this.model.get('Description'),
                Website: this.model.get('Website'),
                Avatar: this.model.get('Avatar'),
                Type: 'Organisation'
            };
        },

        showMenu: function (e) {
            $('.sub-menu-button').removeClass('active');
            $(e.currentTarget).addClass('active');
            e.stopPropagation();
        },

        selectMenuItem: function (e) {
            $('.sub-menu-button').removeClass('active');
            e.stopPropagation();
        },

        startChat: function (e) {
            e.preventDefault();
            app.vent.trigger('chats:joinGroupChat', this.model);
        }
    });

    return SidebarOrganisationItemView;

});