/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// SidebarProjectItemView
// ----------------------

define(['jquery', 'underscore', 'backbone', 'ich', 'app', 'models/project', 'tipsy'],
function ($, _, Backbone, ich, app, Project) {

    var SidebarProjectItemView = Backbone.Marionette.ItemView.extend({
        tagName: 'li',

        className: 'menu-group-item',

        template: 'SidebarProjectItem',

        events: {
            'click .chat-menu-item': 'startChat',
            'click .sub-menu': 'showMenu',
            'click .sub-menu a': 'selectMenuItem'
        },

        initialize: function () {
            this.activityCount = 0;
        },

        serializeData: function () {
            return {
                Id: this.model.id,
                Name: this.model.get('Name'),
                Avatar: this.model.get('Avatar'),
                IsMember: app.authenticatedUser.hasGroupRole(this.model.id, 'roles/projectmember'),
                IsAdministrator: app.authenticatedUser.hasGroupRole(this.model.id, 'roles/projectadministrator')
            };
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

            this.$el.find('#project-menu-group-list .sub-menu a, #project-menu-group-list .sub-menu span').tipsy({ gravity: 'w', live: true });
        },

        showMenu: function (e) {
            app.vent.trigger('close-sub-menus');
            $(e.currentTarget).addClass('active');

            var offsetFromTopOfViewport = $(e.currentTarget).offset().top - $(window).scrollTop();
            var offsetFromLeftOfViewport = ($(e.currentTarget).offset().left - $(window).scrollLeft()) + 25; // 25 = position popup away from arrow
            var windowHeight = $(window).height();
            var $popup = $(e.currentTarget).find('ul');
            var popupHeight = $popup.height();

            log('popupHeight', popupHeight);
            log('windowHeight', windowHeight);
            log('offsetFromTopOfViewport', offsetFromTopOfViewport);

            // if popup is being cut off at bottom of viewport, bring it back up into view
            if (offsetFromTopOfViewport + popupHeight > windowHeight) {
                offsetFromTopOfViewport = windowHeight - popupHeight;
            }

            // if the popup is still too low (ie low enough to be blocked by collapsed chat window, bring it up further
            var chatOverhang = (offsetFromTopOfViewport + popupHeight) - (windowHeight - 50);
            if (chatOverhang > 0) {
                offsetFromTopOfViewport = offsetFromTopOfViewport - chatOverhang;
            }

            $popup.css({
                position: 'fixed',
                top: offsetFromTopOfViewport + 'px',
                left: offsetFromLeftOfViewport + 'px'
            });
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
            app.vent.trigger('chats:joinGroupChat', this.model);
        },

        onNewActivityReceived: function (activity) {
            if (app.authenticatedUser.user.id != activity.get('User').Id) {
                _.each(activity.get('Groups'), function(group) {
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
        }
    });

    return SidebarProjectItemView;

});
