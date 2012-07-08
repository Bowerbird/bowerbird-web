/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// SidebarItemView
// ---------------

define(['jquery', 'underscore', 'backbone', 'app', 'models/project'],
function ($, _, Backbone, app, Project) {

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
            $(this.el).children('a').on('click', function (e) {
                e.preventDefault();
                app.groupUserRouter.navigate($(this).attr('href'), { trigger: true });
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
            e.preventDefault();
            app.vent.trigger('chats:joinGroupChat', this.model);
        },

        observationAdded: function (activity) {
            _.each(activity.get('Groups'), function (group) {
                if (group.Id == this.model.id) {
                    this.activityCount++;
                    if (this.activityCount == 1) {
                        this.$el.find('p').append('<span title=""></span>');
                    }
                    var title = this.activityCount.toString() + ' New Item' + (this.activityCount > 1 ? 's' : '');
                    this.$el.find('p span').text(this.activityCount).attr('title', title);
                }
            },
            this);
        }
    });

    return SidebarProjectItemView;

});
