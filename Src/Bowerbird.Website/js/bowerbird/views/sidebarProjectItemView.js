/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// SidebarItemView
// ---------------

define(['jquery', 'underscore', 'backbone', 'app', 'models/project'], function ($, _, Backbone, app, Project) {

    var SidebarProjectItemView = Backbone.Marionette.ItemView.extend({
        tagName: 'li',

        className: 'menu-group-item',

        template: 'SidebarProjectItem',

        events: {
            'click .chat-menu-item': 'startChat',
            'click .sub-menu-button': 'showMenu',
            'click .sub-menu-button li': 'selectMenuItem'
        },

        initialize: function () {
            this.activityCount = 0;
        },

        onRender: function () {
            var that = this;
            $(this.el).children('a').on('click', function (e) {
                e.preventDefault();
                app.groupUserRouter.navigate($(this).attr('href'));
                app.vent.trigger('project:show:stream', that.model.id);
                return false;
            });

            app.vent.on('newactivity:observationadded', this.observationAdded, this);
        },

        serializeData: function () {
            return {
                Id: this.model.id,
                Name: this.model.get('Name'),
                Avatar: this.model.get('Avatar'),
                Permissions: {
                    UpdateProject: app.authenticatedUser.hasGroupPermission(this.model.id, 'permissions/updateproject')
                }
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
            var chat = app.chats.get(this.sidebarItem.Id);
            if (chat == null) {
                chat = new Bowerbird.Models.GroupChat({ Id: this.sidebarItem.Id, Group: this.sidebarItem });
                app.chats.add(chat);
            }
            app.chatRouter.joinChat(chat);
        },

        observationAdded: function (activity) {
            var self = this;
            _.each(activity.get('Groups'), function (group) {
                if (group.Id == self.model.id) {
                    self.activityCount++;
                    if (self.activityCount == 1) {
                        self.$el.find('p').append('<span></span>');
                    }
                    self.$el.find('p span').text(self.activityCount);
                }
            });
        }
    });

    return SidebarProjectItemView;

});
