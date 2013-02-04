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
                this.$el.find('ul').append(ich.Buttons({ MenuAddSightings: true, Id: this.model.id }));
                
                if (app.authenticatedUser.hasGroupRole(this.model.id, 'roles/projectadministrator')) {
                    this.$el.find('ul').append(ich.Buttons({ MenuAddProjectPost: true, Id: this.model.id }));
                    this.$el.find('ul').append(ich.Buttons({ MenuEditProject: true, Id: this.model.id }));
                }

                this.$el.find('ul').append(ich.Buttons({ MenuChat: true, Id: this.model.id }));
            }

            this.$el.find('#project-menu-group-list .sub-menu a, #project-menu-group-list .sub-menu span').tipsy({ gravity: 'w', live: true });
        },

        serializeData: function () {
            return {
                Id: this.model.id,
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

    return SidebarProjectItemView;

});
