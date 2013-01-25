/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// SidebarItemView
// ---------------

define(['jquery', 'underscore', 'backbone', 'app', 'models/organisation', 'tipsy'],
function ($, _, Backbone, app, Organisation) {

    var SidebarOrganisationItemView = Backbone.Marionette.ItemView.extend({
        tagName: 'li',

        className: 'menu-group-item',

        template: 'SidebarOrganisationItem',

        events: {
            'click .chat-menu-item': 'startChat',
            'click .sub-menu': 'showMenu',
            'click .sub-menu a': 'selectMenuItem'
        },

        initialize: function () {
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

            app.vent.on('newactivity:' + this.model.id + ':postadded', this.onNewActivityReceived, this);

            this.$el.find('#organisation-menu-group-list .sub-menu a, #organisation-menu-group-list .sub-menu span').tipsy({ gravity: 'w', live: true });
        },

        serializeData: function () {
            return {
                Id: this.model.id,
                Name: this.model.get('Name'),
                Avatar: this.model.get('Avatar'),
                Permissions: {
                    UpdateOrganisation: app.authenticatedUser.hasGroupPermission(this.model.id, 'permissions/updateorganisation')
                }
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

        changeSort: function (e) {
            e.preventDefault();
            this.clearListAnPrepareShowLoading();
            Backbone.history.navigate($(e.currentTarget).attr('href'), { trigger: true });
        },

        startChat: function (e) {
            e.preventDefault();
            app.vent.trigger('chats:joinGroupChat', this.model);
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
        }
    });

    return SidebarOrganisationItemView;

});
