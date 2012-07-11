/// <reference path="../../libs/log.js" />
/// <reference path="../../libs/require/require.js" />
/// <reference path="../../libs/jquery/jquery-1.7.2.js" />
/// <reference path="../../libs/underscore/underscore.js" />
/// <reference path="../../libs/backbone/backbone.js" />
/// <reference path="../../libs/backbone.marionette/backbone.marionette.js" />

// NotificationsCompositeView
// --------------------------

// The right hand side bar that is shown to authenticated users.
define(['jquery', 'underscore', 'backbone', 'app', 'views/notificationitemview', 'collections/activitycollection'],
function ($, _, Backbone, app, NotificationItemView, ActivityCollection) {

    var NotificationsCompositeView = Backbone.Marionette.CompositeView.extend({
        tagName: 'section',

        id: 'notifications',

        className: 'triple-3',

        template: 'Notifications',

        itemView: NotificationItemView,

        onRender: function () {
            $('article').append(this.el);
        },

        appendHtml: function (collectionView, itemView) {
            var items = this.collection.pluck('Id');
            var index = _.indexOf(items, itemView.model.id);

            var $li = collectionView.$el.find('ul > li:eq(' + (index) + ')');

            if ($li.length === 0) {
                collectionView.$el.find('ul').append(itemView.el);
            } else {
                $li.before(itemView.el);
            }
        }
    });

    // Initialize the sidebar layout
    app.addInitializer(function (options) {
        // Only show notifications if user is authenticated
        if (app.authenticatedUser) {
            // Setup activity event listening
            var activityCollection = new ActivityCollection();
            activityCollection.baseUrl = '/notifications';
            app.vent.on('newactivity', function (activity) {
                activityCollection.add(activity);
            });

            var notificationsCompositeView = new NotificationsCompositeView({ model: app.authenticatedUser.user, collection: activityCollection });

            notificationsCompositeView.on('show', function () {
                app.vent.trigger('notifications:rendered');
            });

            app.notifications.show(notificationsCompositeView);

            activityCollection.fetchFirstPage();
        }
    });

    return NotificationsCompositeView;

});