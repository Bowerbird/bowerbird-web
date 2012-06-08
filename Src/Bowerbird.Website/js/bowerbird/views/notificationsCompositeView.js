/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// NotificationsCompositeView
// --------------------------

// The right hand side bar that is shown to authenticated users.
define(['jquery', 'underscore', 'backbone', 'app', 'views/notificationitemview'],
function ($, _, Backbone, app, NotificationItemView) {

    var NotificationsCompositeView = Backbone.Marionette.CompositeView.extend({
        tagName: 'section',

        id: 'notifications',

        className: 'triple-3',

        template: 'Notifications',

        itemView: NotificationItemView,

        onRender: function () {
            $('article').append(this.el);

            app.vent.on('newactivity', this.newActivity, this);
        },

        appendHtml: function(collectionView, itemView){
            collectionView.$el.find('ul').prepend(itemView.el);
        },

        newActivity: function(activity) {
        }
    });

    // Initialize the sidebar layout
    app.addInitializer(function (options) {
        $(function () {
            // Only show notifications if user is authenticated
            if (app.authenticatedUser) {
                // Render the layout and get it on the screen, first
                var notificationsCompositeView = new NotificationsCompositeView({ model: app.authenticatedUser.user, collection: app.activities });

                notificationsCompositeView.on('show', function () {
                    app.vent.trigger('notifications:rendered');
                });

                app.notifications.show(notificationsCompositeView);
            }
        });
    });

    return NotificationsCompositeView;

});