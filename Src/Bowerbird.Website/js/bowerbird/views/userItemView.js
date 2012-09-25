/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// OnlineUserItemView
// ------------------

define(['jquery', 'underscore', 'backbone', 'app', 'models/user'],
function ($, _, Backbone, app) {

    var UserItemView = Backbone.Marionette.ItemView.extend({

        tagName: 'li',

        className: 'online-user-item',

        template: 'UserItem',

        events: {
            'click .online-user-item a': 'startChat',
            'click .chat-menu-item': 'startChat',
            'click .sub-menu-button': 'showMenu',
            'click .sub-menu-button li': 'selectMenuItem'
        },

        initialize: function () {
            _.bindAll(this, 'onStatusChange', 'onUpdateUserStatus', 'onRender');
            this.model.on('statuschange', this.onStatusChange, this);
            if (this.model.id === app.authenticatedUser.user.id) {
                this.model.on('pollserver', this.onUpdateUserStatus, this);
            }
        },

        serializeData: function () {
            return {
                Model: {
                    User: this.model.toJSON(),
                    ShowChat: this.model.id !== app.authenticatedUser.user.id
                }
            };
        },

        startChat: function (e) {
            if (this.model.getCurrentStatus() != 'offline') {
                app.vent.trigger('chats:startPrivateChat', this.model);
            }
        },

        onRender: function () {
            log('userItemView.onRender');
            var self = this;
            this.onStatusChange({ user: this.model, status: this.model.getCurrentStatus() });
            self.model.startTimer();
            // if this user is 'Me' track my interactivity to pass back to the server
            if (self.model.id === app.authenticatedUser.user.id) {
                self.model.startTracker();
            }
        },

        showMenu: function (e) {
            $('.sub-menu-button').removeClass('active');
            $(e.currentTarget).addClass('active');
            var position = $(e.currentTarget).offset();
            var scrollTop = $(window).scrollTop();
            this.$el.find('.sub-menu-button ul').css({ position: 'fixed' }).css({ top: position.top - scrollTop + 'px', left: position.left + 25 + 'px' });
            e.stopPropagation();
        },

        selectMenuItem: function (e) {
            $('.sub-menu-button').removeClass('active');
            e.stopPropagation();
        },

        onStatusChange: function (userStatus) {
            var chatMenuCss = {};
            var actualStatus = '';
            if (userStatus.status === 'online') {
                actualStatus = 'online';
                chatMenuCss.display = 'list-item';
            } else if (userStatus.status === 'away') {
                actualStatus = 'away';
                chatMenuCss.display = 'list-item';
            } else if (userStatus.status === 'offline') {
                actualStatus = 'offline';
                chatMenuCss.display = 'none';
            }
            this.$el.find('.chat-menu').css(chatMenuCss);
            this.$el.find('.user-name')
                .removeClass('online')
                .removeClass('away')
                .removeClass('offline')
                .addClass(actualStatus)
                .empty()
                .html(userStatus.user.get('Name') + ' <span>' + actualStatus + '</span>');
            log(this.model.get('Name') + ' is now ' + actualStatus);
        },

        onUpdateUserStatus: function (userStatus) {
            log('userItemView:onUpdateUserStatus:');
            if (userStatus.user.id === app.authenticatedUser.user.id) {
                app.userHubRouter.updateUserClientStatus(userStatus.user.id, userStatus.user.get('LatestHeartbeat'), userStatus.user.get('LatestActivity'));
            }
        }
    });

    return UserItemView;

});
