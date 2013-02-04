/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// SidebarUserProjectItemView
// --------------------------

define(['jquery', 'underscore', 'backbone', 'ich', 'app', 'models/userproject', 'tipsy'],
function ($, _, Backbone, ich, app, UserProject) {

    var SidebarUserProjectItemView = Backbone.Marionette.ItemView.extend({
        tagName: 'li',

        className: 'menu-group-item',

        template: 'SidebarUserProjectItem',

        events: {
            'click .chat-menu-item': 'startChat',
            'click .sub-menu': 'showMenu',
            'click .sub-menu a': 'selectMenuItem'
        },

        initialize: function () {
            _.bindAll(this, 'onStatusChange');

            this.activityCount = 0;
        },

        onRender: function () {
            var that = this;

            this.$el.children('a').on('click', function (e) {
                e.preventDefault();
                Backbone.history.navigate($(this).attr('href'), { trigger: true });
                that.activityCount = 0;
                that.$el.find('p span').remove();
                return false;
            });

            app.vent.on('newactivity:' + this.model.id + ':sightingadded newactivity:' + this.model.id + ':postadded newactivity:' + this.model.id + ':sightingnoteadded', this.onNewActivityReceived, this);

            if (app.authenticatedUser) {
                log('chat.....................', this.model.get('CreatedBy'), app.onlineUsers.get(this.model.get('CreatedBy')));

                app.onlineUsers.on('add', function (item) {
                    if (item.id === that.model.get('CreatedBy')) {
                        that.chatUser = item;
                        that.chatUser.on('statuschange', this.onStatusChange, this);
                        that.$el.find('ul').append(ich.Buttons({ MenuChat: true, Id: that.model.id }));
                    }
                });
                app.onlineUsers.on('remove', function (item) {
                    if (item.id === that.model.get('CreatedBy')) {
                        that.chatUser = null;
                        that.chatUser.off('statuschange');
                        that.$el.find('ul .chat-menu-item').remove();
                    }
                });
            }

            this.$el.find('#userproject-menu-group-list .sub-menu a, #userproject-menu-group-list .sub-menu span').tipsy({ gravity: 'w', live: true });
        },

        serializeData: function () {
            return {
                Id: this.model.get('CreatedBy'),
                Name: this.model.get('Name'),
                Avatar: this.model.get('Avatar')
            };
        },

        showMenu: function (e) {
            app.vent.trigger('close-sub-menus');
            $(e.currentTarget).addClass('active');
            e.stopPropagation();
        },

        selectMenuItem: function (e) {
            e.preventDefault();
            app.vent.trigger('close-sub-menus');
            Backbone.history.navigate($(e.currentTarget).attr('href'), { trigger: true });
            return false;
        },

        startChat: function (e) {
            e.preventDefault();
            app.vent.trigger('close-sub-menus');
            if (this.chatUser.getCurrentStatus() != 'offline') {
                app.vent.trigger('chats:startPrivateChat', this.chatUser);
            }
            return false;
        },

        onNewActivityReceived: function (activity) {
            _.each(activity.get('Groups'), function (group) {
                if (group.Id === this.model.id) {
                    this.activityCount++;
                    if (this.activityCount == 1) {
                        this.$el.find('p').append('<span title=""></span>');
                    }
                    var title = this.activityCount.toString() + ' New Item' + (this.activityCount > 1 ? 's' : '');
                    this.$el.find('p span').text(this.activityCount).attr('title', title);
                }
            },
            this);
        },

        onStatusChange: function (userStatus) {
            if (userStatus.status === 'online') {
                this.$el.find('.chat-menu-item').show();
            } else if (userStatus.status === 'away') {
                this.$el.find('.chat-menu-item').show();
            } else if (userStatus.status === 'offline') {
                this.$el.find('.chat-menu-item').hide();
            }
        }
    });

    return SidebarUserProjectItemView;

});
